var dataTable;

$(document).ready(function () {
    // Way to get URL by using window.location.seach;
    var url = window.location.search;

    if (url.includes("inprocess")) {
        loadDataTable("inproccess");
    }
    else
    {
        if (url.includes("pending")) {
            loadDataTable("pending");
        }
        else
        {
            if (url.includes("completed")) {
                loadDataTable("completed");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    } 
});

// When we are passing teh order get all, we also need to pass the status here in ajax route
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "10%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "%15" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/admin/order/details?orderId=${data}" class="btn btn-primary m-3">
                    <i class="bi bi-pencil-square"></i>
                    </a>`
                },
                "width": "15%"
            }
        ]
    });
}