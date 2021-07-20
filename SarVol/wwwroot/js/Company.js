var dataTable;

$(document).ready(function () { loadDataTable(); });



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url:"/Admin/Company/GetAll"
        },
        "columns": [

            { data: "name", width: "10%", class: "text-dark" },
            { data: "streetAdress", width: "15%", class: "text-dark" },
            { data: "city", width: "15%", class: "text-dark" },
            { data: "phoneNumber", width: "15%", class: "text-dark" },
            { data: "country", width: "15%", class: "text-dark" },
            { data: "companyDiscount", width: "15%", class: "text-dark" },
            {
                data: "id",
                "render":function(data) {
                    return `<div class="text-center">
                                <a class="btn btn-success text-white"  href="/Admin/Company/Upsert/${data}" style="cursor:pointer">
                                           <i class="fas fa-pen-alt"></i>
                                </a>
                                <a class="btn btn-danger text-white" onclick=Delete("/Admin/Company/Delete?id=${data}")  style="cursor:pointer">
                                    <i class="far fa-trash-alt"></i>
                                </a>
                            </div>`
                },width:"15%"
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