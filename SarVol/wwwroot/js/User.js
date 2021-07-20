var dataTable;

$(document).ready(function () { loadDataTable(); });



function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url:"/Admin/User/GetAll"
        },
        "columns": [

            { data: "name", width:"15%",class:"text-dark" },
            { data: "email", width:"15%",class:"text-dark" },
            { data: "phoneNumber", width:"15%",class:"text-dark" },
            { data: "company.name", width:"15%",class:"text-dark" },
            { data: "role", width:"15%",class:"text-dark" },
            {
                data: {id:"id",lockoutEnd:"lockoutEnd"},
                "render": function (data)
                {
                    var today = new Date().getTime();
                    var lock = new Date(data.lockoutEnd).getTime()
                    if (lock > today) {
                        //user is currently locked
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock-open"></i>  Unlock
                                </a>
                            </div>
                           `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-unlock-alt"></i> Lock
                                </a>
                            </div>
                           `;
                    }

                   

                   
                },width:"25%"
            },


        ]


    });
}
function LockUnlock(id)
{

   
            $.ajax({
                type: "POST",
                url: '/Admin/User/LockUnlock',
                data: JSON.stringify(id),
               
                contentType:"application/json",
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


