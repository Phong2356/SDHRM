document.addEventListener("DOMContentLoaded", function () {
    // 1. Khai báo các biến
    const toggleBtn = document.getElementById("toggleSidebar");
    const mainSidebar = document.querySelector(".sidebar-main, .sidebar-main-employee, .sidebar-main-timesheet");
    const subSidebar = document.getElementById("sub-sidebar");

    // Lấy TẤT CẢ các nút có menu con thay vì chỉ 1 nút ID cố định
    const menuHasSubs = document.querySelectorAll(".menu-has-sub");
    // Lấy TẤT CẢ các nhóm menu con bên trong sub-sidebar
    const subMenuGroups = document.querySelectorAll(".sub-menu-group");

    // 2. Xử lý sự kiện click nút "Thu gọn" Menu chính
    if (toggleBtn && mainSidebar) {
        toggleBtn.addEventListener("click", function () {
            // Toggle class 'collapsed' cho sidebar chính
            mainSidebar.classList.toggle("collapsed");

            // Nếu đang thu gọn menu chính, thì đóng luôn sidebar phụ cho gọn
            if (mainSidebar.classList.contains("collapsed") && subSidebar) {
                subSidebar.classList.remove("open"); // Tắt class open của sidebar phụ
                subSidebar.style.display = "none";   // Ẩn đi (dự phòng nếu CSS chưa có class open)

                // Bỏ trạng thái active của tất cả các nút menu
                menuHasSubs.forEach(btn => btn.classList.remove("active"));

                // Ẩn tất cả nội dung menu con
                subMenuGroups.forEach(group => group.style.display = "none");
            }
        });
    }

    // 3. Xử lý mở/đóng cho ĐA dạng Menu con (Chấm công, Đơn từ...)
    if (menuHasSubs.length > 0 && subSidebar) {
        menuHasSubs.forEach(btn => {
            btn.addEventListener("click", function (e) {
                e.preventDefault(); // Ngăn trình duyệt load lại trang khi bấm href="#"

                // Nếu sidebar chính đang bị thu gọn, mở nó ra trước
                if (mainSidebar && mainSidebar.classList.contains("collapsed")) {
                    mainSidebar.classList.remove("collapsed");
                }

                // Lấy ID của khối menu con tương ứng từ thuộc tính data-target (VD: #submenu-dontu)
                const targetId = this.getAttribute("data-target");
                const targetGroup = document.querySelector(targetId);

                // Kiểm tra xem nút vừa bấm có đang được mở không
                const isCurrentlyOpen = this.classList.contains("active");

                // Bước 3.1: Reset tất cả về trạng thái đóng trước
                menuHasSubs.forEach(b => b.classList.remove("active"));
                subMenuGroups.forEach(group => group.style.display = "none");

                if (isCurrentlyOpen) {
                    // Nếu đang mở thì đóng lại hoàn toàn
                    subSidebar.classList.remove("open");
                    subSidebar.style.display = "none";
                } else {
                    // Nếu đang đóng thì mở cái tương ứng lên
                    this.classList.add("active"); // Làm nổi bật nút ở menu chính
                    if (targetGroup) targetGroup.style.display = "flex"; // Hiển thị nhóm menu con

                    subSidebar.classList.add("open"); // Thêm class open (nếu CSS của bạn cần)
                    subSidebar.style.display = "block"; // Hiển thị khối sidebar phụ
                }
            });
        });
    }

    // 4. (Bổ sung UX) Khi bấm vào các menu KHÔNG có menu con (Tổng quan, Hồ sơ) -> Đóng menu phụ lại
    const regularLinks = document.querySelectorAll('.nav-link:not(.menu-has-sub):not(#toggleSidebar)');
    regularLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (subSidebar) {
                subSidebar.classList.remove("open");
                subSidebar.style.display = "none";
            }
            menuHasSubs.forEach(b => b.classList.remove("active"));
            subMenuGroups.forEach(group => group.style.display = "none");
        });
    });
});