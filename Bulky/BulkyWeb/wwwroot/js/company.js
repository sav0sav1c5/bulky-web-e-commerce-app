var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/company/getAll' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'streetAddress', "width" : "15%"},
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "10%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'postalCode', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="d-flex justify-content-center gap-2">
                                <a href="/admin/company/upsert?id=${data}" class="btn btn-primary w-50 d-flex align-items-center justify-content-center">
                                    <i class="bi bi-pencil-square me-2"></i> Edit
                                </a>
                                <a onClick="Delete('/admin/company/delete/${data}')" class="btn btn-danger w-50 d-flex align-items-center justify-content-center rounded-5">
                                    <i class="bi bi-trash-fill me-2"></i> Delete
                                </a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    })
}