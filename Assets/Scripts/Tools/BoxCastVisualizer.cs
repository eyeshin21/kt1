using UnityEngine;

public class BoxCastVisualizer : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(1, 1, 1);  // Kích thước của hộp
    public float castDistance = 10f;                // Khoảng cách box cast
    public LayerMask collisionMask;                 // Layer để kiểm tra va chạm

    void OnDrawGizmos()
    {
        // Tính bán kính của hộp để dùng cho BoxCast
        Vector3 halfExtents = boxSize * 0.5f;
        // Thực hiện BoxCast
        RaycastHit hit;
        bool isHit = Physics.BoxCast(transform.position, halfExtents, transform.forward,
                                     out hit, transform.rotation, castDistance, collisionMask);

        if (isHit)
        {
            // Nếu hit, vẽ hộp ở điểm va chạm (có thể cần điều chỉnh vị trí để phù hợp với nhu cầu)
            Gizmos.color = Color.red;
            // Tính vị trí để vẽ hộp: vị trí hit cộng thêm một chút dịch chuyển nếu cần
            Vector3 hitBoxCenter = transform.position + transform.forward * hit.distance;
            Gizmos.DrawWireCube(hitBoxCenter, boxSize);
        }
        else
        {
            // Nếu không hit, vẽ hộp tại vị trí cast tối đa
            Gizmos.color = Color.green;
            Vector3 endPoint = transform.position + transform.forward * castDistance;
            Gizmos.DrawWireCube(endPoint, boxSize);
        }

        // Tùy chọn: Vẽ đường ray từ vị trí bắt đầu đến điểm kết thúc
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * castDistance);
    }
}
