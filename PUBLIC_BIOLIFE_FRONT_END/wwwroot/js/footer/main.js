$(document).ready(function () {

    footer.bin_data();
})

var footer = {
    bin_data: function () {

        $.ajax({
            dataType: 'html', // Nếu bạn trả về HTML
            type: 'POST', // Hoặc 'POST' nếu controller là POST
            url: '/home/loadFooterComponent',
            success: function (data) {

                $('#footer-container').html(data);
            },
            error: function (xhr, status, error) {
                console.log("Error: " + error); // Thay đổi từ 'failure' sang 'error'
            }
        });

    }
}