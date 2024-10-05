$(document).ready(function () {
    
    header.bind_menu();
})

var header = {
    bind_menu: function () {
      
        $.ajax({
            dataType: 'html', // Nếu bạn trả về HTML
            type: 'POST', // Hoặc 'POST' nếu controller là POST
            url: '/home/loadHeaderComponent',
            success: function (data) {
               
                $('#header-container').html(data);
            },
            error: function (xhr, status, error) {
                console.log("Error: " + error); // Thay đổi từ 'failure' sang 'error'
            }
        });

    }
}