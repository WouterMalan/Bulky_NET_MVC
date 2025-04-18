var dataTable;

$(document).ready(() => {
    const url = window.location.search;
    
    const orderStatuses = ['inprocess', 'completed', 'pending', 'approved'];
    
    const status = orderStatuses.find(status => url.includes(status)) || 'all';
    
    loadDataTable(status);
});

// Load DataTable
function loadDataTable(status) {
    dataTable = $('#orderTable').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status
        },
        "columns": [
            { data: 'id', width: '25%' },
            { data: 'name', width: '20%' },
            { data: 'phoneNumber', width: '20%' },
            { data: 'applicationUser.email', width: '20%' },
            { data: 'orderStatus', width: '10%' },
            { data: 'orderTotal', width: '10%' },
            {
                data: 'category', 
                width: '10%',
                "render": function(data, type, row) {
                    return data ? data.name : '';
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/order/details?orderId=${data}" class="btn btn-success mx-2" style="cursor:pointer;">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "10%"
            }
        ]
    });
}
