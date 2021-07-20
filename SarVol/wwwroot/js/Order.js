var dataTable;

$(document).ready(function ()
{
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("?status=inprocess")
    }
    else if (url.includes("pending"))
    {

        loadDataTable("?status=pending")
    }
    else if (url.includes("completed")) {

        loadDataTable("?status=completed")
    }
    else if (url.includes("rejected"))
    {
        loadDataTable("?status=rejected")
    }
    else
    {
        loadDataTable("?status=all");
    }

  

});



function loadDataTable(url) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: "/Admin/Order/GetOrders" + url
        },
        "columns": [

            { data: "id", width:"15%",class:"text-dark" },
            { data: "name", width:"15%",class:"text-dark" },
            { data: "phoneNumber", width:"10%",class:"text-dark" },
            { data: "appUser.email", width:"10%",class:"text-dark" },
            { data: "orderStatus", width:"10%",class:"text-dark" },
            { data: "paymentStatus", width:"10%",class:"text-dark" },
            { data: "orderTotal", width:"15%",class:"text-dark" },
            {
                data: "id",
                "render":function(data) {
                    return `
                                <div class="text-center">
                                    <a class="btn btn-success text-white"  href="/Admin/Order/Details/${data}" style="cursor:pointer">
                                     Details
                                 </a>
                            </div>`;
                },width:"10%"
            }


        ]


    });
}
