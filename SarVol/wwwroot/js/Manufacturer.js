var dataTable;

$(document).ready(function () { loadDataTable(); });



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: "/Admin/Manufactures/GetAll"
        },
        "columns": [

            { data: "name", width: "60%", class: "text-dark" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a class="btn btn-success text-white"  href="/Admin/Manufactures/Upsert/${data}" style="cursor:pointer">
                                           <i class="fas fa-pen-alt"></i>
                                </a>
                                <a class="btn btn-danger text-white" onclick=Delete("/Admin/Manufactures/Delete?id=${data}")  style="cursor:pointer">
                                   <i class="far fa-trash-alt"></i>
                                </a>
                            </div>`
                }, width: "40%"
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
        dangerMode: true

    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success)
                    {

                       
                        toastr.options = {
                            "closeButton": false,
                            "debug": false,
                            "newestOnTop": false,
                            "progressBar": false,
                            "positionClass": "toast-top-right",
                            "preventDuplicates": false,
                            "onclick": null,
                            "showDuration": "300",
                            "hideDuration": "1000",
                            "timeOut": "5000",
                            "extendedTimeOut": "1000",
                            "showEasing": "swing",
                            "hideEasing": "linear",
                            "showMethod": "fadeIn",
                            "hideMethod": "fadeOut"
                        }
                        toastr["success"](data.message);

                       
                       
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