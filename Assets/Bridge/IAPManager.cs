using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
#if UNITY_ANDROID
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.MiniJSON; // có trong Unity IAP
#endif

namespace CGTeam
{
    public class IAPManager
    {
        //public static string kRemoveAds = "adone_diytidy_removeads";
        public static string kStarterPack = "ndm_hookscrew_starterpack";

        public static string kCoin1 = "ndm_hookscrew_coin_1";
        public static string kCoin2 = "ndm_hookscrew_coin_2";
        public static string kCoin3 = "ndm_hookscrew_coin_3";
        public static string kCoin4 = "ndm_hookscrew_coin_4";
        public static string kCoin5 = "ndm_hookscrew_coin_5";
        public static string kCoin6 = "ndm_hookscrew_coin_6";
        public enum State { PendingInitialize, Initializing, SuccessfullyInitialized, FailedToInitialize };

        private static IAPManager m_instance = null;
        public static IAPManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new IAPManager();

                return m_instance;
            }
        }

        private State m_initializationState = State.PendingInitialize;
        public State InitializationState { get { return m_initializationState; } }
        public bool IsInitialized { get { return m_initializationState == State.SuccessfullyInitialized; } }

        public delegate void InitializationCallback(bool success);
        private InitializationCallback m_onInitialized;
        public event InitializationCallback OnInitialized
        {
            add
            {
                if (m_initializationState == State.SuccessfullyInitialized || m_initializationState == State.FailedToInitialize)
                    value?.Invoke(m_initializationState == State.SuccessfullyInitialized);
                else
                    m_onInitialized += value;
            }
            remove { m_onInitialized -= value; }
        }

        public delegate void CompletedPurchaseCallback(Product product);
        public CompletedPurchaseCallback OnPurchaseCompleted;

        public delegate void FailedPurchaseCallback(Product product, PurchaseFailureReason failureReason);
        public FailedPurchaseCallback OnPurchaseFailed;

        public delegate void NativeIAPWindowClosedCallback();
        private NativeIAPWindowClosedCallback onIAPWindowClosed;

        public delegate void NativeRestoreWindowClosedCallback(bool success);
        private NativeRestoreWindowClosedCallback onRestoreWindowClosed;

        // ===== IAP v5 objects =====
        private StoreController store;                 // entry point mới
        private CatalogProvider catalogProvider;       // nếu dùng IAP Catalog
        private readonly HashSet<string> ownedNonConsumables = new HashSet<string>();

#if UNITY_ANDROID
        private CrossPlatformValidator androidValidator;   // local-validate
#endif

        #region Initialize
        // Dùng IAP Catalog trong Editor (khuyên dùng)
        public void Initialize()
        {
            InitializeInternal(useCatalog: true, null);
        }

        // Tự truyền danh sách ProductDefinition (không dùng Catalog)
        public void Initialize(params ProductDefinition[] products)
        {
            InitializeInternal(false, (IEnumerable<ProductDefinition>)products);
        }
        public void Initialize(IEnumerable<ProductDefinition> products)
        {
            InitializeInternal(false, products);
        }

        private async void InitializeInternal(bool useCatalog, IEnumerable<ProductDefinition> products)
        {
            if (m_initializationState != State.PendingInitialize)
            {
                Debug.LogWarning("[IAP] Already initializing/initialized.");
                return;
            }

            try
            {
                m_initializationState = State.Initializing;

                // 1) Tạo StoreController & gắn events
                store = new StoreController();
                WireEvents();

                // 2) (Android) Chuẩn bị validator local
#if UNITY_ANDROID
                try
                {
                    androidValidator = new CrossPlatformValidator(
                        GooglePlayTangle.Data(), // cần GooglePlayTangle.cs
                        null,                    // Apple tangle: không dùng trong v5
                        Application.identifier);
                }
                catch (Exception e) { Debug.LogWarning("[IAP] Android validator init warn: " + e.Message); }
#endif

                // 3) Connect
                await store.Connect();

                // 4) Fetch products
                if (useCatalog)
                {
                    // Lấy product list từ IAP Catalog (Window → In-App Purchasing → IAP Catalog)
                    var catalog = ProductCatalog.LoadDefaultCatalog();
                    if (catalog == null || catalog.allProducts == null || catalog.allProducts.Count == 0)
                    {
                        Debug.LogWarning("[IAP] IAP Catalog trống hoặc không tải được.");
                    }
                    else
                    {
                        var defs = new List<ProductDefinition>(catalog.allProducts.Count);
                        foreach (var p in catalog.allProducts)
                        {
                            // p.id phải khớp với SKU trên store; p.type là ProductType.Consumable/NonConsumable/Subscription
                            defs.Add(new ProductDefinition(p.id, p.type));
                        }
                        store.FetchProducts(defs);
                    }
                }
                else
                {
                    if (products == null)
                    {
                        m_initializationState = State.FailedToInitialize;
                        m_onInitialized?.Invoke(false);
                        return;
                    }

                    List<ProductDefinition> tempProducts = new List<ProductDefinition>();
                    foreach (ProductDefinition productDefinition in products)
                    {
                        tempProducts.Add(productDefinition);
                    }

                    store.FetchProducts(tempProducts);
                }

                m_initializationState = State.SuccessfullyInitialized;
                m_onInitialized?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[IAP] Initialize failed: " + e);
                m_initializationState = State.FailedToInitialize;
                m_onInitialized?.Invoke(false);
            }
        }
        #endregion

        #region Price Product
        public string GetProductCurrencyFromStore(string id)
        {
            var p = store?.GetProductById(id);
            return p?.metadata?.isoCurrencyCode ?? "";
        }
        public string GetProductPriceStringFromStore(string id)
        {
            var p = store?.GetProductById(id);
            return p?.metadata?.localizedPriceString ?? "";
        }
        public decimal GetProductPriceFromStore(string id)
        {
            var p = store?.GetProductById(id);
            return p?.metadata?.localizedPrice ?? 0m;
        }
        #endregion

        #region Purchase / Restore
        public void Purchase(string productID, NativeIAPWindowClosedCallback onIAPWindowClosed = null)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAP] Not initialized, cannot purchase.");
                onIAPWindowClosed?.Invoke();
                return;
            }

            this.onIAPWindowClosed = onIAPWindowClosed;

            var product = store.GetProductById(productID);
            if (product == null)
            {
                Debug.LogWarning("[IAP] Product not found: " + productID);
                OnPurchaseFailed?.Invoke(null, PurchaseFailureReason.ProductUnavailable);
                OnNativeIAPWindowClosed();
                return;
            }

            // IAP 5: dùng Cart để Play có thể hiển thị selector quantity (nếu hỗ trợ)
            var cart = new Cart(new CartItem(product));
            store.Purchase(cart);
        }

        public void RestorePurchases(NativeRestoreWindowClosedCallback onRestoreWindowClosed = null)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("[IAP] Not initialized, cannot restore.");
                onRestoreWindowClosed?.Invoke(false);
                return;
            }

            this.onRestoreWindowClosed = onRestoreWindowClosed;
            store.RestoreTransactions((success, msg) =>
            {
                Debug.Log($"[IAP] Restore result: {success} ({msg})");
                OnNativeRestoreWindowClosed(success);
            });
        }

        public bool IsNonConsumablePurchased(string productID)
        {
            if (!IsInitialized) return false;

            foreach (var order in store.GetPurchases())
            {
                foreach (var item in order.CartOrdered.Items())
                {
                    var p = item.Product;
                    if (p != null && p.definition.id == productID &&
                        p.definition.type != ProductType.Consumable)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region Store events
        private void WireEvents()
        {
            store.OnProductsFetched += OnProductsFetched;
            store.OnProductsFetchFailed += OnProductsFetchFailed;

            store.OnPurchasesFetched += OnPurchasesFetched;
            store.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

            store.OnPurchasePending += OnPurchasePending;       // grant ở đây → Confirm
            store.OnPurchaseConfirmed += OnPurchaseConfirmed;   // log
            store.OnPurchaseFailed += OnPurchaseFailedInternal; // fail
        }

        private bool initialPurchasesFetchRequested = false;
        private void OnProductsFetched(List<Product> products)
        {
            Debug.Log($"[IAP] Products fetched: {products?.Count ?? 0}");

            // Chỉ fetch purchases lần đầu sau khi đã có product details
            if (!initialPurchasesFetchRequested)
            {
                initialPurchasesFetchRequested = true;
                store.FetchPurchases();
            }
        }

        private void OnProductsFetchFailed(ProductFetchFailed failed)
        {
            var ids = (failed.FailedFetchProducts != null && failed.FailedFetchProducts.Count > 0)
                ? string.Join(",", failed.FailedFetchProducts.ConvertAll(d => d.id))
                : "(none)";
            Debug.LogWarning($"[IAP] FetchProducts FAILED: {failed.FailureReason}; ids: {ids}");
            // Không hạ state về fail để app vẫn tiếp tục (tuỳ chọn)
        }

        private void OnPurchasesFetched(Orders orders)
        {
            ownedNonConsumables.Clear();
            Debug.Log($"[IAP] Purchases fetched: {orders?.ConfirmedOrders.Count ?? 0}");

            bool canCallOnCompleteEvent = false;

            if (PlayerPrefs.GetInt("auto_restore", 0) == 0)
            {
                // Dùng chung logic với mua mới
                canCallOnCompleteEvent = true;
            }

            if (orders != null)
            {
                foreach (var order in orders.ConfirmedOrders)
                {
                    Debug.Log($"[IAP] Order Tx={order.Info?.TransactionID}");
                    foreach (var item in order.CartOrdered.Items())
                    {
                        var p = item.Product;
                        if (p == null) continue;

                        Debug.Log($"[IAP]  Product={p.definition.id}, type={p.definition.type}");

                        if (p.definition.type != ProductType.Consumable)
                        {
                            ownedNonConsumables.Add(p.definition.id);

                            if (canCallOnCompleteEvent)
                            {
                                PlayerPrefs.SetInt("auto_restore", 1);
                                PlayerPrefs.Save();

                                // Nếu muốn flow restore dùng chung logic với mua mới:
                                try
                                {
                                    OnPurchaseCompleted?.Invoke(p);   // gọi hàm grant
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription fail)
        {
            Debug.LogWarning($"[IAP] FetchPurchases FAILED: {fail.FailureReason} - {fail.message}");
        }

        private void OnPurchaseConfirmed(Order order)
        {
            // Logging/analytics nếu cần
            Debug.Log($"[IAP] Confirmed. Tx={order?.Info?.TransactionID}");
        }

        private void OnPurchaseFailedInternal(FailedOrder failed)
        {
            try
            {
                Product any = null;
                if (failed.CartOrdered.Items().Count > 0)
                    any = failed.CartOrdered.Items()[0].Product;

                Debug.LogWarning($"[IAP] Purchase FAILED: {failed.Details}");
                OnPurchaseFailed?.Invoke(any, PurchaseFailureReason.Unknown);
            }
            finally
            {
                OnNativeIAPWindowClosed();
            }
        }

        // ====== Nơi “ăn” multi-quantity + Android local-validate ======
        private void OnPurchasePending(PendingOrder pending)
        {
            try
            {
                // (1) Android local-validate receipt (nếu có validator)
#if UNITY_ANDROID
                if (androidValidator != null)
                {
                    try
                    {
                        // Ném IAPSecurityException nếu receipt/signature sai
                        androidValidator.Validate(pending.Info?.Receipt);
                    }
                    catch (IAPSecurityException ex)
                    {
                        Debug.LogWarning("[IAP] Android local validation FAILED: " + ex.Message);
                        // Không confirm → giữ pending để lần sau re-deliver
                        // bắn callback fail cho game:
                        Product any = null;
                        if (pending.CartOrdered.Items().Count > 0)
                            any = pending.CartOrdered.Items()[0].Product;
                        OnPurchaseFailed?.Invoke(any, PurchaseFailureReason.SignatureInvalid);
                        return;
                    }
                }
#endif
                // Gộp item theo SKU rồi tính qty
                var totals = new Dictionary<string, (Product p, int qty)>();
                foreach (var it in pending.CartOrdered.Items())
                {
                    var p = it.Product;
                    if (p == null) continue;

                    int addQty = Mathf.Max(1, it.Quantity);   // nhiều bản IAP 5 vẫn trả 1
#if UNITY_ANDROID
                    // Thử đọc quantity thật từ receipt Google (nếu lớn hơn 1 thì ưu tiên)
                    var qFromReceipt = GetGoogleReceiptQuantity(pending.Info?.Receipt);
                    if (qFromReceipt > addQty) addQty = qFromReceipt;
#endif
                    if (!totals.TryGetValue(p.definition.id, out var t))
                        totals[p.definition.id] = (p, addQty);
                    else
                        totals[p.definition.id] = (p, t.qty + addQty);
                }

                // Grant + Confirm
                foreach (var kv in totals)
                {
                    Product p = kv.Value.p;
                    int qty = kv.Value.qty;

                    // Track AppsFlyer (1 lần cho mỗi item hoặc theo chiến lược của bạn)
                    //try { AppsFlyerObjectPurchaseScript.AppsFlyerPurchaseEvent(p, qty); } catch { }

                    for (int i = 0; i < qty; i++)
                    {
                        OnPurchaseCompleted?.Invoke(p);
                    }

                    if (p.definition.type != ProductType.Consumable)
                        ownedNonConsumables.Add(p.definition.id);
                }

                // (3) Quan trọng: Confirm để kết thúc giao dịch
                store.ConfirmPurchase(pending);
            }
            finally
            {
                OnNativeIAPWindowClosed();
            }
        }

#if UNITY_ANDROID
        private int GetGoogleReceiptQuantity(string receiptJson)
        {
            try
            {
                // Unity IAP bọc receipt kiểu: { "Store":"GooglePlay", "TransactionID":"...", "Payload":"{\"json\":\"{...}\",\"signature\":\"...\"}" }
                var outer = Json.Deserialize(receiptJson) as Dictionary<string, object>;
                if (outer == null) return 1;

                // Payload là chuỗi JSON chứa { json: "...json gốc của Google...", signature: "..." }
                var payloadStr = outer.TryGetValue("Payload", out var pVal) ? pVal as string : null;
                if (string.IsNullOrEmpty(payloadStr)) return 1;

                var payload = Json.Deserialize(payloadStr) as Dictionary<string, object>;
                if (payload == null) return 1;

                var innerJsonStr = payload.TryGetValue("json", out var jVal) ? jVal as string : null;
                if (string.IsNullOrEmpty(innerJsonStr)) return 1;

                // JSON gốc của Google Billing: có thể có "quantity"
                var gp = Json.Deserialize(innerJsonStr) as Dictionary<string, object>;
                if (gp != null && gp.TryGetValue("quantity", out var qObj) && qObj != null)
                {
                    // quantity có thể là long/double/string -> ép về int an toàn
                    if (qObj is long l) return (int)l;
                    if (qObj is double d) return (int)d;
                    if (int.TryParse(qObj.ToString(), out var qi)) return Mathf.Max(1, qi);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[IAP] Parse Google receipt quantity failed: " + e.Message);
            }
            return 1; // mặc định
        }
#endif
        #endregion

        private void OnNativeIAPWindowClosed()
        {
            try
            {
                onIAPWindowClosed?.Invoke();
                onIAPWindowClosed = null;
            }
            catch (Exception e) { Debug.LogException(e); }
        }

        private void OnNativeRestoreWindowClosed(bool success)
        {
            try
            {
                onRestoreWindowClosed?.Invoke(success);
                onRestoreWindowClosed = null;
            }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}