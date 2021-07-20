var dataTable;

$(document).ready(function () { loadDataTable(); });



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url:"/Admin/Product/GetAll"
        },
        "columns": [


            { data: "productName", width: "15%", class: "text-dark" },
            { data: "regularPrice", width: "15%", class: "text-dark" },
            { data: "promotionalPrice", width: "15%", class: "text-dark" },
            { data: "category.name", width: "15%", class: "text-dark" },
            { data: "taste.name", width: "10%", class: "text-dark"},
            { data: "manufacturer.name", width: "10%", class: "text-dark" },
            {
                data: "id",
                "render":function(data) {
                    return `<div class="text-center">
                                <a class="btn btn-success text-white"  href="/Admin/Product/Upsert/${data}" style="cursor:pointer">
                                            <i class="fas fa-pen-alt"></i>
                                </a>
                                <a class="btn btn-danger text-white" onclick=Delete("/Admin/Product/Delete?id=${data}")  style="cursor:pointer">
                                    <i class="far fa-trash-alt"></i>
                                </a>
                            </div>`
                },width:"20%"
            }


        ]


    });
}
function Delete(url) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data",
        icon: "warning",
        buttons: true,
        dangerMode:true

    }).then((willDelete) =>
    {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data)
                {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }

                }
            })
        }


    });
}