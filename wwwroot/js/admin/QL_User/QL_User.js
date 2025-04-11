document.addEventListener("DOMContentLoaded", function () {
    //=============================================================================
    // --------------------------Hàm tìm kiếm người dùng---------------------------
    //=============================================================================
    function searchUsers() {
        let keyword = document.getElementById("searchBox").value.trim();
        let table = document.querySelector(".table tbody");
        
        if (keyword === "") {
            showMessage("Vui lòng nhập từ khóa tìm kiếm!");
            return;
        }

        fetch(`/Admin/SearchUser?email=${keyword}`)
            .then(response => response.json())
            .then(data => {
                table.innerHTML = "";
                if (data.success) {
                    data.users.forEach(user => {
                        table.innerHTML += `
                            <tr id="row_${user.PK_iUserID}" data-id="${user.PK_iUserID}" 
                                data-name="${user.sUserName}" data-email="${user.sEmail}" 
                                data-dob="${user.dNS ? user.dNS.split("T")[0] : ""}" 
                                data-cccd="${user.sCCCD}" data-phone="${user.sSDT}">
                                <td>${user.PK_iUserID}</td>
                                <td class="user-name">${user.sUserName}</td>
                                <td class="user-email">${user.sEmail}</td>
                                <td class="user-dob">${user.dNS ? user.dNS.split("T")[0] : ""}</td>
                                <td class="user-cccd">${user.sCCCD}</td>
                                <td class="user-phone">${user.sSDT}</td>
                                <td>
                                    <button class="btn-delete" data-id="${user.PK_iUserID}">Xóa</button>
                                </td>
                            </tr>`;
                    });
                } else {
                    showMessage(data.message);
                }
            })
            .catch(error => showMessage("Đã xảy ra lỗi khi tìm kiếm."));
    }

    //=========================================================================
    // ---------------------------Hàm xóa người dùng---------------------------
    //=========================================================================
    function deleteUser(userId) {
        if (!confirm("Bạn có chắc muốn xóa người dùng này không?")) return;
    
        fetch(`/Admin/DeleteUser?id=${userId}`, { method: "POST" })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    document.getElementById(`row_${userId}`).remove();
                    showMessage("Xóa thành công!");
                } else {
                    showMessage("Xóa thất bại!");
                }
            })
            .catch(error => console.error("Lỗi khi xóa:", error));
    }

    document.querySelector(".table tbody").addEventListener("click", function (event) {
        if (event.target.classList.contains("btn-delete")) {
            let userId = event.target.getAttribute("data-id");
            deleteUser(userId);
        }
    });

    //=================================================================
    // ------------------Hàm kiểm tra hợp lệ dữ liệu-------------------
    //=================================================================
    function validateInput(userData) {
        let nameRegex = /^[a-zA-ZÀ-ỹ\s]{3,50}$/;
        let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        let cccdRegex = /^\d{12}$/;
        let phoneRegex = /^0\d{9}$/;
        let passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/;
        let minAge = 16;
        let birthDate = new Date(userData.dNS);
        let today = new Date();
        let age = today.getFullYear() - birthDate.getFullYear();
        let monthDiff = today.getMonth() - birthDate.getMonth();
        if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }

        if (!nameRegex.test(userData.sUserName)) {
            showMessage("Họ tên không hợp lệ.");
            return false;
        }
        if (!emailRegex.test(userData.sEmail)) {
            showMessage("Email không hợp lệ.");
            return false;
        }
        if (age < minAge) {
            showMessage("Người dùng phải đủ 16 tuổi.");
            return false;
        }
        if (!cccdRegex.test(userData.sCCCD)) {
            showMessage("CCCD phải có 12 chữ số.");
            return false;
        }
        if (!phoneRegex.test(userData.sSDT)) {
            showMessage("Số điện thoại không hợp lệ.");
            return false;
        }
        if (userData.sPW && !passwordRegex.test(userData.sPW)) {
            showMessage("Mật khẩu ít nhất 6 ký tự, gồm cả chữ và số.");
            return false;
        }
        return true;
    }

    //=========================================================================
    // ---------------------------Hàm thêm người dùng---------------------------
    //=========================================================================
    function addUser() {
        let userData = {
            sUserName: document.getElementById("name").value.trim(),
            sEmail: document.getElementById("email").value.trim(),
            dNS: document.getElementById("dob").value,
            sCCCD: document.getElementById("cccd").value.trim(),
            sSDT: document.getElementById("phone").value.trim(),
            sPW: document.getElementById("password").value.trim()
        };

        if (!validateInput(userData)) return;

        fetch('/Admin/AddUser', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showMessage("Thêm người dùng thành công!");
                location.reload();
            } else {
                showMessage("Thêm thất bại: " + data.message);
            }
        })
        .catch(error => showMessage("Lỗi hệ thống!"));
    }

    //=========================================================================
    // ---------------------------Hàm cập nhật người dùng---------------------------
    //=========================================================================

    // Chọn người dùng và đẩy dữ liệu lên input
    function selectUser(row) {
        document.querySelectorAll("tr").forEach(tr => tr.classList.remove("selected"));
        row.classList.add("selected");
    
        document.getElementById("name").value = row.dataset.name || "";
        document.getElementById("email").value = row.dataset.email || "";
        document.getElementById("dob").value = row.dataset.dob || "";
        document.getElementById("cccd").value = row.dataset.cccd || "";
        document.getElementById("phone").value = row.dataset.phone || "";
        document.getElementById("password").value = ""; 
    }

    // Làm mới ô input sau khi cập nhật thành công
    function resetInputs() {
        document.getElementById("name").value = "";
        document.getElementById("email").value = "";
        document.getElementById("dob").value = "";
        document.getElementById("cccd").value = "";
        document.getElementById("phone").value = "";
        document.getElementById("password").value = "";
    }
    
    function saveUserChanges() {
        let selectedRow = document.querySelector("tr.selected");
        if (!selectedRow) {
            showMessage("Vui lòng chọn một người dùng để chỉnh sửa!");
            return;
        }
    
        let userId = selectedRow.dataset.id;
        let updatedUser = {
            PK_iUserID: userId,
            sUserName: document.getElementById("name").value.trim(),
            sEmail: document.getElementById("email").value.trim(),
            dNS: document.getElementById("dob").value || null,
            sCCCD: document.getElementById("cccd").value.trim(),
            sSDT: document.getElementById("phone").value.trim(),
            sPW: document.getElementById("password").value.trim() || null
        };
        
        if (!validateInput(updatedUser)) return;
        
        fetch(`/Admin/UpdateUser/${userId}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updatedUser)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                showMessage("Cập nhật thành công!");
                updateTableRow(selectedRow, updatedUser);
                resetInputs();
            } else {
                showMessage("Cập nhật thất bại: " + data.message);
            }
        })
        .catch(error => showMessage("Đã xảy ra lỗi khi cập nhật!"));
    }
    
    function showMessage(message) {
        alert(message);
    }

    function updateTableRow(row, updatedUser) {
        row.dataset.name = updatedUser.sUserName;
        row.dataset.email = updatedUser.sEmail;
        row.dataset.dob = updatedUser.dNS || "";
        row.dataset.cccd = updatedUser.sCCCD;
        row.dataset.phone = updatedUser.sSDT;
    
        row.children[1].innerText = updatedUser.sUserName;
        row.children[2].innerText = updatedUser.sEmail;
        row.children[3].innerText = updatedUser.dNS || "";
        row.children[4].innerText = updatedUser.sCCCD;
        row.children[5].innerText = updatedUser.sSDT;
    }
    
    window.searchUsers = searchUsers;
    window.deleteUser = deleteUser;
    window.addUser = addUser;
    window.saveUserChanges = saveUserChanges;

    document.getElementById("saveButton").addEventListener("click", saveUserChanges);
    document.querySelectorAll("table tbody tr").forEach(row => {
        row.addEventListener("click", function (event) {
            if (!event.target.classList.contains("btn-delete")) {
                selectUser(this);
            }
        });
    });
});