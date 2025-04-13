// We are loading DataTable here but it needs to be done on document.ready
$(document).ready(function () {
    loadDataTable();
});

// This function load DataTable and in ajax needs to be the path to the controller action
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { "data": 'title', "width": "25%" },
            { "data": 'isbn', "width": "15%" },
            { "data": 'price', "width": "10%" },
            { "data": 'author', "width": "15%" },
            { "data": 'category.name', "width": "10%" },
            {
                "data": 'id',
                "render": function (data) {
                    return `<div class="d-flex justify-content-center gap-2">
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-secondary w-50">
                                    <i class="bi bi-pencil-square me-2"></i>
                                    Edit
                                </a>
                                
                                <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-secondary bg-danger w-50">
                                    <i class="bi bi-pencil-square me-2"></i>
                                    Delete
                                </a>
                            </div>`
                },
                "width": "25%"
            }
        ]

    });
}

// Function to be called when the delete button is clicked
// It will get URL that needs to be 
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            // We want to make Ajax request to our controller action to delete Product
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    // If it's successful we will need to reload changed Product list in database (refresh table)
                    dataTable.ajax.reload()
                    // And we will have function where we will get data back
                    toastr.success(data.message);
                }
            })
            //Swal.fire({
                //title: "Deleted!",
                //text: "Your file has been deleted.",
                //icon: "success"
            //});
        }
    });
}