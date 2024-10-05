//$(document).ready(function () {
//    global_service.Initialization();
//    global_service.DynamicBind();
//    global_service.LoadPolicy();
//    global_service.LoadAbouHulotoys();
//    global_service.LoadCustomerSupport();
//    global_service.LoadCartCount();
//})
var global_service = {
    Initialization: function () {
        if (window.history && window.history.pushState) {
            $(window).on('popstate', function () {
                window.location.reload()
            });

        }
        $('#thanhcong').removeClass('overlay-active')
        $('#thatbai').removeClass('overlay-active')
        $('#dangnhap').removeClass('overlay-active')
        $('#dangky').removeClass('overlay-active')
        $('#quenmk').removeClass('overlay-active')
    },
    DynamicBind: function () {
        $("body").on('click', ".client-login", function (event) {
            var element = $(this)
            event.preventDefault()
            var box_id = element.attr('data-id')
            $('.client-login-popup').removeClass('overlay-active')
            $('' + box_id).addClass('overlay-active')
        });
        $("body").on('click', ".overlay .close, .overlay .btn-close", function (event) {
            var element = $(this)
            event.preventDefault()
            element.closest('.overlay').removeClass('overlay-active')
        });
        //$("body").on('keyup', ".global-search", function (event) {
        //    var element = $(this)
        //    global_service.RenderSearchBoxLoading()
        //    if (element.val() != undefined && element.val().trim() != '') {
        //        $('.box-search-list').fadeIn()
        //        global_service.RenderSearchBox()
        //    } else {
        //        $('.box-search-list').fadeOut()
        //    }
            
        //});
        $("body").on('keyup', ".global-search", global_service.DelayEventBinding(function (e) {
            var element = $(this)
            global_service.RenderSearchBoxLoading()
            if (element.val() != undefined && element.val().trim() != '') {
                $('.box-search-list').fadeIn()
                global_service.RenderSearchBox()
            } else {
                $('.box-search-list').fadeOut()
            }
        }, 800));
    },
    LoadPolicy: function () {
        $.ajax({
            url: "/Support/GetListPolicy",
            type: 'post',
            data: { idTypePolicy : 21},
            success: function (data) {
                data.forEach(item => {
                    let html = `<li><a class="li-Cursor" onclick="global_service.PolicyNaviga('/chinh-sach/','${item.id}','${item.name}')">${item.name}</a></li>`;
                    $(".policy-footer").append(html);
                });
            },
        });
    },
    LoadAbouHulotoys: function () {
        $.ajax({
            url: "/Support/GetListAboutHulotoys",
            type: 'post',
            data: {idCate : 25},
            success: function (data) {
                data.forEach(item => {
                    let html = `<li><a class="li-Cursor" onclick="global_service.Naviga('/tin-tuc/','${item.id}','${item.title}-${item.id}')">${item.title}</a></li>`;
                    $(".AboutHulotoy-footer").append(html);
                });
            },
        });
    },
    LoadCustomerSupport: function () {
        $.ajax({
            url: "/Support/GetListCustomerSupport",
            type: 'post',
            data: {idCate : 26},
            success: function (data) {
                data.forEach(item => {
                    let html = `<li><a class="li-Cursor" onclick="global_service.Naviga('/tin-tuc/','${item.id}','${item.title}-${item.id}')">${item.title}</a></li>`;
                    $(".CustomerSupport-footer").prepend(html);
                });
            },
        });
    },
    LoadCartCount: function () {
        var usr = global_service.CheckLogin()
        if (usr) {
            $.ajax({
                url: API_URL.CartCount,
                type: 'post',
                data: {
                    request: {
                        account_client_id: usr.account_client_id
                    }
                },
                success: function (result) {
                    if (result.is_success && result.data) {
                        $('#carts .badge').html(result.data)
                    } else { $('#carts .badge').html('0') }
                },
            });
        }
        else {
            $('#carts .badge').html('0')
        }
       
    },
    PolicyNaviga: function (url,id,title)
    {
        window.location.href = url + this.convertVietnameseToUnsign(title) + "-" + id;
        //localStorage.setItem('ChosenIdPolicy', id);
        //localStorage.setItem('ChosenUrlPolicy', title);
    },
    Naviga: function (url, id, title) {
        window.location.href = url + this.convertVietnameseToUnsign(title);
    },
    CheckLogin: function () {
        var str = localStorage.getItem(STORAGE_NAME.Login)
        if (str != undefined && str.trim() != '') {
            return JSON.parse(str)
        }
        str = sessionStorage.getItem(STORAGE_NAME.Login)
        if (str != undefined && str.trim() != '') {
            return JSON.parse(str)
        }
        return undefined
    },
    POST: function (url, data) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: 'post',
                url: url,
                data: { request: data },
                success: function (data) {
                    resolve(data);
                },
                error: function (err) {
                    reject(err);
                }
            });
        });
    },


    GET: function (url) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                url: url,
                dataType: 'json',
                type: 'get',
                contentType: 'application/json',
                processData: false,
                success: function (data) {
                    resolve(data);
                },
                error: function (err) {
                    reject(err);
                }
            });
        });
    },
    DecodeGSIToken: function (token) {
        let base64Url = token.split('.')[1]
        let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        let jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload)
    },
    Comma: function (number) { //function to add commas to textboxes
        number = ('' + number).replace(/[^0-9.,]+/g, '');
        number += '';
        number = number.replaceAll(',', '');
        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1))
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        return x1 + x2;
    },
    RemoveUnicode: function ( text) {
        var arr1 =  [
            "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ",
            "í", "ì", "ỉ", "ĩ", "ị",
            "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ",
            "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự",
            "ý", "ỳ", "ỷ", "ỹ", "ỵ"];
        var arr2 =  [
            "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e",
            "i", "i", "i", "i", "i",
            "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o",
            "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u",
            "y", "y", "y", "y", "y"];
        for (var i = 0; i < arr1.length; i++) {
            text = text.replaceAll(arr1[i], arr2[i]);
            text = text.replaceAll(arr1[i].toUpperCase(), arr2[i].toUpperCase());
        }
        return text;
    },
    convertVietnameseToUnsign: function (str)
    {
       // Bảng chuyển đổi các ký tự có dấu thành không dấu
       const from = "àáạảãâầấậẩẫăắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơớờợởỡùúụủũưừứựửữỳýỵỷỹđ";
       const to = "aaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd";
       const fromArray = from.split('');
       const toArray = to.split('');

       str = str.toLowerCase();

       for (let i = 0; i < fromArray.length; i++)
       {
       str = str.replace(new RegExp(fromArray[i], 'g'), toArray[i]);
       }

       str = str.replace(/\s+/g, '-'); // Thay thế nhiều khoảng trắng thành 1 -
       return str.trim();
    },
    LoadHomeProductGrid: function (element, group_id, size) {
        element.addClass('placeholder')
        element.addClass('box-placeholder')
        element.css('width', '100%')
        element.css('height', '255px')
        var request = {
            "group_id": group_id,
            "page_index": 1,
            "page_size": size
        }
        $.when(
            global_service.POST(API_URL.ProductList, request)
        ).done(function (result) {

            var html = ''
            if (result.is_success) {
                html = global_service.RenderSlideProductItem(result.data, HTML_CONSTANTS.Home.SlideProductItem)
            }
            element.html(html)
            element.removeClass('placeholder')
            element.removeClass('box-placeholder')
            element.css('height', 'auto')
        })
    },
    GotoCart: function () {
        var usr = global_service.CheckLogin()
        if (usr) {
            window.location.href='/cart'

        }
        else {
            $('.mainheader .client-login').click()
            return
        }
    },
  
    DateTimeToString: function (date, has_time=false) {
        var text = ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear() ;
        if (has_time == true) {
            text += + ' ' + ("0" + date.getHours()).slice(-2) + ':' + ("0" + date.getMinutes()).slice(-2)
        }
        return text
    },
    DateTimeDotNetToString: function (date_string, has_time = false) {
        //"2024-08-28T09:15:09.43Z"
        var date = new Date(date_string)
        var text = ("0" + date.getDate()).slice(-2) + '/' + ("0" + (date.getMonth() + 1)).slice(-2) + '/' + date.getFullYear() ;
        if (has_time == true) {
            var time_text = + ' ' + (date.getHours()) + ':' + (("0" + date.getMinutes()).slice(-2))
            return text + ' ' + time_text
        }
        return text
    },
    CorrectImage: function (image) {
         var img_src = image
         if (img_src == null || img_src == undefined) return ''
         if (!img_src.includes(API_URL.StaticDomain)
            && !img_src.includes("data:image")
            && !img_src.includes("http")
            && !img_src.includes("base64,"))
             img_src = API_URL.StaticDomain + image
        return img_src
    },
    Select2WithFixedOptionAndSearch: function (element, placeholder = "Vui lòng chọn...") {
      
        element.select2({
            placeholder: placeholder,
        });
    },
    
    RemoveSpecialCharacters: function(value) {
        value = value.replace(/[^a-zA-Z0-9 ]/g, '');
        return value.trim();
    },
    RenderSearchBox: function () {
        var usr = global_service.CheckLogin()
        var account_client_id = 0
        if (usr) {
            account_client_id = usr.account_client_id
           
        }
        var request = {
            "keyword": $('.global-search').val(),
            "account_client_id": account_client_id
        }
        $.when(
            global_service.POST(API_URL.GlobalSearch, request)
        ).done(function (result) {
            if (result.is_success && result.data && result.data.items) {
                if (result.data.items.length > 0) {
                    var html = `<div class="list-product-recomment">` + global_service.RenderSlideProductItem(result.data.items, HTML_CONSTANTS.Home.GlobalSearchProductItem) +`</div>`
                    $('.box-search-list').html(html)
                    return
                } 
            }
            $('.box-search-list').html('Không tìm thấy kết quả')

        })
     
    },
    RenderSearchBoxLoading: function () {
        $('.box-search-list').html(HTML_CONSTANTS.Home.GlobalSearchBoxLoading)
        $('.box-search-list .item-product').addClass('placeholder')
        $('.box-search-list .flex-price').addClass('placeholder')
        $('.box-search-list .name-product').addClass('placeholder')
        $('.box-search-list .price-old').addClass('placeholder')
    },
    RenderSlideProductItem: function (list, template) {
        var html = ''
        $(list).each(function (index, item) {
            var img_src = item.avatar
            if (!img_src.includes(API_URL.StaticDomain)
                && !img_src.includes("data:image")
                && !img_src.includes("http"))
                img_src = API_URL.StaticDomain + item.avatar
            var amount_html = 'Giá liên hệ'
            var has_price = false
            if (item.amount_max != undefined
                && item.amount_max != null
                && item.amount_min != undefined
                && item.amount_min != null) {
                amount_html = global_service.Comma(item.amount_min) + ' - ' + global_service.Comma(item.amount_max)
                has_price = true
            }
            else if (item.amount != undefined
                && item.amount != null && item.amount > 0) {
                amount_html = global_service.Comma(item.amount)
                has_price = true
            }
            if (has_price) {
                html += template
                    .replaceAll('{url}', '/san-pham/' + global_service.RemoveUnicode(global_service.RemoveSpecialCharacters(item.name)).replaceAll(' ', '-') + '--' + item._id)
                    .replaceAll('{avt}', img_src)
                    .replaceAll('{name}', item.name)
                    .replaceAll('{amount}', amount_html)
                    //.replaceAll('{review_point}', (item.rating == null || item.rating == undefined || item.rating <= 0) ? '5' : item.rating)
                    .replaceAll('{review_point}', '5')
                    .replaceAll('{review_count}', '')
                    .replaceAll('{old_price_style}', '')
                    .replaceAll('{price}', (item.amount == null || item.amount == undefined || item.amount <= 0) ? global_service.Comma(item.amount) + ' đ' : '')

            }
        });

        return html
    },
    DelayEventBinding: function (callback, ms) {
        var timer = 0;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () {
                callback.apply(context, args);
            }, ms || 0);
        };
    }

}