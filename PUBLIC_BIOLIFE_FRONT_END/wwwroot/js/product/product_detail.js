$(document).ready(function () {
    product_detail.Initialization();  
    product_detail.save_product_history_local(); // product view
})
var product_detail = {
    
    save_product_history_local: function () {
       
        var prod_history = JSON.parse($("#product_json_detail").val());
        debugger;
        if (prod_history.amount_vnd != "") {
            var list_result = [];
            var j_list_hist = localStorage.getItem(PRODUCT_HISTORY);
            if (j_list_hist == null) {
                //add first list
                list_result.push(prod_history);
                localStorage.setItem(PRODUCT_HISTORY, JSON.stringify(list_result));
            } else {
                var obj_prod_hist = JSON.parse(j_list_hist);

                //add pro
                // check pro in list
                var list_result = obj_prod_hist.filter(function (el) { return el.product_code == prod_history.product_code; });
                if (list_result.length == 0) {
                    if (obj_prod_hist.length > LIMIT_PRODUCT_HIST) {
                        //remove 1 pro first
                        obj_prod_hist.splice(obj_prod_hist.length - 1, obj_prod_hist.length);
                    }
                    //add first list                    
                    obj_prod_hist.unshift(prod_history);                    
                    //save
                    localStorage.setItem(PRODUCT_HISTORY, JSON.stringify(obj_prod_hist));
                } else {

                }
            }
        }
    },
    Initialization: function () {

        sessionStorage.removeItem(STORAGE_NAME.ProductDetail)
        sessionStorage.removeItem(STORAGE_NAME.SubProduct)
        product_detail.Detail()
        product_detail.DynamicBind()
    },
    DynamicBind: function () {
        $("body").on('click', ".attribute-detail", function () {
            var element = $(this)
            if (!element.hasClass('disabled')) {
                element.closest('ul').find('li').removeClass('active')
                element.addClass('active')
            }
            var product = product_detail.GetProductDetailSession()
            if (product != undefined) {
                product_detail.RenderChangedAttributeSelected(product)

            } else {
                window.location.reload()
            }
        });
    },
    Detail: function () {
        var code = $('.section-details-product').attr('data-code')
        if (code == undefined || code.trim() == '')
            window.location.href = '/error'
        var request = {
            "id": code
        }
        $.when( // click atribute
            global_service.POST(API_URL.GetProductDetail, request)
        ).done(function (result) {
            if (result.is_success && result.data && result.data.product_main) {
                sessionStorage.setItem(STORAGE_NAME.ProductDetail, JSON.stringify(result.data))
                sessionStorage.setItem(STORAGE_NAME.SubProduct, JSON.stringify(result.data.product_sub))
            }
        })
    },
    RenderDetail: function (product, product_sub) {
        var html = ''
        var img_src = product.avatar
        img_src = global_service.CorrectImage(product.avatar)


        html += HTML_CONSTANTS.Detail.Images
            .replaceAll('{src}', img_src)

        $(product.images).each(function (index, item) {

            img_src = global_service.CorrectImage(item)
            html += HTML_CONSTANTS.Detail.Images
                .replaceAll('{src}', img_src)

        });
        $('.section-details-product .gallery-product .swiper-wrapper').html(html)

        $('.section-details-product .name-product').html(product.name)
        if (product.product_sold_count == undefined || product.product_sold_count <= 0) {
            $('.section-details-product .total-sold').hide()
        } else {
            $('.section-details-product .total-sold').html(global_service.Comma(product.product_sold_count) + ' Đã bán')
        }
        if (product.reviews_count == undefined || product.reviews_count <= 0) {
            $('.section-details-product .total-sold').hide()
        } else {
            $('.section-details-product .total-review').html(global_service.Comma(product.reviews_count) + ' Đánh giá')
        }


        html = ''
        for (var i = 0; i < (product.star <= 0 ? 5 : product.star); i++) {
            html += HTML_CONSTANTS.Detail.Star
        }
        html += '' + (product.star <= 0 ? 5 : product.star)
        $('.box-review .review').html(html)

        if (product_sub != undefined && product_sub.length > 0) {
            const max_obj = product_sub.reduce(function (prev, current) {
                return (prev && prev.amount > current.amount) ? prev : current
            })
            const min_obj = product_sub.reduce(function (prev, current) {
                return (prev && prev.amount < current.amount) ? prev : current
            })
            if (max_obj.amount <= min_obj.amount)
                $('.section-details-product .price').html(global_service.Comma(min_obj.amount))
            else
                $('.section-details-product .price').html(global_service.Comma(min_obj.amount) + ' - ' + global_service.Comma(max_obj.amount))
        }
        else {
            $('.section-details-product .price').html(global_service.Comma(product.amount))

        }
        if (product.discount > 0) {
            $('#price-old').html(global_service.Comma(product.amount + product.discount))
        } else {
            $('#price-old').closest('.price-old').hide()
        }
        var total_stock = product.quanity_of_stock

        html = ''
        //html += HTML_CONSTANTS.Detail.Tr_Voucher.replaceAll('{span}', '')
        //html += HTML_CONSTANTS.Detail.Tr_Combo.replaceAll('{span}', '')
        //html += HTML_CONSTANTS.Detail.Tr_Shipping
        //html += HTML_CONSTANTS.Detail.Tr_Combo.replaceAll('{span}', '')
        if (product_sub != undefined && product_sub.length > 0) {
            $(product.attributes).each(function (index, attribute) {
                var attr_detail = product.attributes_detail.filter(obj => {
                    return obj.attribute_id === attribute._id
                })
                var html_item = ''
                if (attr_detail != undefined && attr_detail.length > 0) {
                    $(attr_detail).each(function (index_detail, attribute_detail) {
                        img_src = global_service.CorrectImage(attribute_detail.img)

                        html_item += HTML_CONSTANTS.Detail.Tr_Attributes_Td_li
                            .replaceAll('{active}', '')
                            .replaceAll('{src}', attribute_detail.img != undefined && attribute_detail.img.trim() != '' ? '<img src="' + img_src + '" />' : '')
                            .replaceAll('{name}', attribute_detail.name)
                    })
                }
                html += HTML_CONSTANTS.Detail.Tr_Attributes
                    .replaceAll('{level}', attribute._id)
                    .replaceAll('{name}', attribute.name)
                    .replaceAll('{li}', html_item)
            });
            total_stock = product_sub.reduce((n, { amount }) => n + amount, 0)
        }
        //html += HTML_CONSTANTS.Detail.Tr_Quanity.replaceAll('{stock}', global_service.Comma(total_stock))

        $('.box-info-details .attribute-table').html(html)
        $('.section-details-product .box-description .content').html(product.description.replaceAll('\n', '<br />'))

        $('#combo-discount .list-product .item-product').each(function (index, item) {
            var element = $(this)
            if (index < 5) return true
            else element.hide()
        })
        html = ''
        $(product.specification).each(function (index, specification) {
            if (specification.value != null && specification.value != 'null' && specification.value != undefined && specification.value.trim() != '') {
                var spec_name = GLOBAL_CONSTANTS.DefaultSpecificationValue.filter(obj => {
                    return obj.id === specification.attribute_id
                })
                var template = `  <tr>
                                <td>Thương hiệu</td>
                                <td>Las Palms</td>
                            </tr>`
                html += template.replaceAll('Thương hiệu', spec_name[0].name)
                    .replaceAll('Las Palms', specification.value)
            }
        });
        $('#tb-info-table tbody').html(html)
        //product_detail.RenderBuyNowButton()
        product_detail.RenderSavedProductDetailAttributeSelected()
        product_detail.RemoveLoading()
    },


    GetProductDetailSession: function () {
        var json = sessionStorage.getItem(STORAGE_NAME.ProductDetail)
        if (json != undefined && json.trim() != '') {
            return JSON.parse(json)
        }
        return undefined
    },
    GetSubProductSessionByAttributeSelected: function () {
        var json = sessionStorage.getItem(STORAGE_NAME.SubProduct)
        if (json != undefined && json.trim() != '') {
            var list = JSON.parse(json)
            var sub_list = list
            var options = []
            $('.box-info-details .attribute-table .attributes').each(function (index, item) {
                var element = $(this)
                var value = element.find('.box-tag').find('.active').attr('data-id')
                var level = element.attr('data-level')
                options.push({
                    id: level,
                    name: value
                })

            })
            $(options).each(function (index, item) {

                sub_list = sub_list.filter(({ variation_detail }) =>
                    variation_detail.some(v => v.id == item.id && v.name == item.name)
                )


            })
            return sub_list[0]
        }
        return undefined
    },
    RenderChangedAttributeSelected: function (product) {
        var options = []
        $('.box-info-details .attribute-table .attributes').each(function (index, item) {
            var element = $(this)
            var value = element.find('.box-tag').find('.active').attr('data-id')
            var level = element.attr('data-level')
            options.push({
                id: level,
                name: value
            })
        })
        var json = sessionStorage.getItem(STORAGE_NAME.SubProduct)
        if (json != undefined && json.trim() != '') {
            var list = JSON.parse(json)
            var variation = list.filter(obj => {
                return product_detail.Compare2Array(obj.variation_detail, options)
            })
            if (variation != undefined && variation.length > 0) {
                $('.section-details-product .price').html(global_service.Comma(variation[0].amount))
                $('.box-info-details .box-detail-stock .soluong').html(global_service.Comma(variation[0].quanity_of_stock) + ' sản phẩm có sẵn')
            }
        }

    },
    RenderSavedProductDetailAttributeSelected: function () {
        var selected = sessionStorage.getItem(STORAGE_NAME.ProductDetailSelected)
        if (selected) {
            var data = JSON.parse(selected)
            if (data != undefined) {
                if (data.attributes != undefined && data.attributes.length > 0) {
                    $('.box-info-details .attributes').each(function (index_detail, attribute_detail) {
                        var tr_attributes = $(this)
                        var level = tr_attributes.attr('data-level')
                        var selected_value = data.attributes.filter(obj => {
                            return obj.level == level
                        })

                        tr_attributes.find('li').each(function (index_detail, attribute_detail) {
                            var li_attribute = $(this)
                            if (li_attribute.attr('data-id') == selected_value[0].data) {
                                li_attribute.trigger('click')
                                return false
                            }

                        })
                    })
                }
                if (data.quanity != undefined && data.quanity.trim() != '') {
                    $('.quantity').val(data.quanity)
                }
            }
            sessionStorage.removeItem(STORAGE_NAME.ProductDetailSelected)
        }


    },
    Compare2Array: function (arr1, arr2) {
        const objectsEqual = (o1, o2) =>
            Object.keys(o1).length === Object.keys(o2).length
            && Object.keys(o1).every(p => o1[p] === o2[p]);
        const arraysEqual = (a1, a2) =>
            a1.length === a2.length && a1.every((o, idx) => objectsEqual(o, a2[idx]));
        return arraysEqual(arr1, arr2)

    }
}

