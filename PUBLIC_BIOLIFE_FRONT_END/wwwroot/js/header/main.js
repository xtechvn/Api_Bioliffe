$(document).ready(function () {

    header.bind_menu();
    header.get_product_history();
})

var header = {
    //box san pham da xem
    get_product_history: function () {

        var j_list_hist = localStorage.getItem(PRODUCT_HISTORY);
        if (j_list_hist !== null) {

            var prod_history = JSON.parse(j_list_hist);
            var list_result = prod_history;

            //if (list_result.length >= LIMIT_PRODUCT_HIST) {
            // append view

            var productListJson = list_result.slice(0, LIMIT_PRODUCT_HIST);

            var section_product = `               
                    <div class="container">
                        <div class="list-product-category">
                            <div class="title-box">
                                <h3 class="name-tt"><a href="">Sản phẩm đã xem </a></h3>                
                            </div>
                            <div class="product-category">
                                <div class="list-product scroll-product">
                                        {productListContainer}
                                </div>
                            </div>
                        </div>
                    </div>               
            `;

            // Lặp qua danh sách sản phẩm và thêm vào HTML
            var productListContainer = "";               
            productListJson.forEach(function (product) {
                productListContainer += header.createProductItem(product);
            });

            $(".product_history").html(section_product.replace("{productListContainer}", productListContainer));            
        } else {
            $(".product_history").remove();
        }
    },
    // Hàm tạo HTML cho mỗi sản phẩm
    createProductItem: function (product) {

        return `
        <div class="item-product">
            <a href="${product.link}">
                <div class="thumb thumb-product">
                    <img src="${product.img_thumb}" alt="${product.product_name}" />
                </div>
                <div class="box-info">
                    <h3 class="name-product">${product.product_name}</h3>
                    <div class="flex-price">
                        <div class="price-sale">${product.amount_vnd}</div>
                    </div>
                </div>
            </a>
        </div>
    `;
    },
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