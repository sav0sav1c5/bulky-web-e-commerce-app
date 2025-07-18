﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getAll' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'company.name', "width": "10%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'role', "width": "15%" },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `<div class="text-center d-flex justify-content-center gap-2">
                                    <a onclick="LockUnlock('${data.id}')" class="btn btn-danger text-white flex-fill" style="cursor: pointer;">
                                        <i class="bi bi-unlock-fill"></i> Lock
                                    </a>
                                    <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white flex-fill" style="cursor: pointer;">
                                        <i class="bi bi-pancil-square"></i> Permission
                                    </a>
                                </div>`
                    }
                    else
                    {
                        return `<div class="text-center  d-flex justify-content-center gap-2">
                                    <a onclick="LockUnlock('${data.id}')" class="btn btn-success text-white flex-fill" style="cursor: pointer;">
                                        <i class="bi bi-unlock-fill"></i> Unlock
                                    </a>
                                    <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white flex-fill" style="cursor: pointer;">
                                        <i class="bi bi-pancil-square"></i> Permission
                                    </a>
                                </div>`
                    }
                },
                "width": "20%"
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}