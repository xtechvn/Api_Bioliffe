const swiperBanner = new Swiper('.swiper-banner', {
    loop: true,
    pagination: {
        el: '.swiper-banner .swiper-pagination',
    },
    navigation: {
        nextEl: '.swiper-banner .swiper-button-next',
        prevEl: '.swiper-banner .swiper-button-prev',
    },

});
const swiperADS = new Swiper('.banner-cat', {
    loop: false,
    pagination: false,
    navigation: false,
    slidesPerView: 1.5,
    spaceBetween: 8,
    breakpoints: {
        540: {
            slidesPerView: 1.5,
        },
        768: {
            slidesPerView: 2.5,
        },
        992: {
            slidesPerView: 4,
        }
    },
    
});

const swiperAds = new Swiper('.swiper-banner-ads', {
    loop: false,
    pagination: false,
    navigation: false,
    slidesPerView: 1,
    spaceBetween: 8,
    breakpoints: {
        540: {
            slidesPerView: 1,
        },
        992: {
            slidesPerView: 2,
        }
    }
});

const swiperClient = new Swiper('.swiper-client', {
    loop: false,
    pagination: false,
    navigation: false,
    slidesPerView: 3,
    spaceBetween: 8,
    breakpoints: {
        540: {
            slidesPerView: 3,
        },
        768: {
            slidesPerView: 4.5,
        },
        992: {
            slidesPerView: 6.5,
        }
    }
});
const swiperFlash = new Swiper('.section-flashsale .product-slide', {
    loop: false,
    pagination: false,
    navigation: false,
    spaceBetween: 15,
    slidesPerView: 1.5,
    breakpoints: {
        540: {
            slidesPerView: 2.5,
        },
        768: {
            slidesPerView: 3.5,
        },
        1024: {
            slidesPerView: 4.5,
        },
        1400: {
            slidesPerView: 5,
        }
    },
    navigation: {
        nextEl: '.product-slide .swiper-button-next',
        prevEl: '.product-slide .swiper-button-prev',
    },
});
$(function () {
    $('.btn-filter').on('click', function () {
        $('#productList').addClass('show-filter');
    });
    $('#closeFilter').on('click', function () {
        $('#productList').removeClass('show-filter');
    });
    $('.list-tab-menu .sub-menu').on('click', function () {
        $(this).toggleClass('active');
    });
    // stiky menu
    var stickyElements = $('.sticky');
    if (stickyElements.length > 0) {
        Stickyfill.add(stickyElements);
    }
    $("<div id='menu_before'></div>").insertBefore(".header");
    $(window).scroll(function () {
        var top_start = $("#menu_before").position().top + 0;
        if ($(window).scrollTop() > top_start) {
            $('.header').addClass('pin');
        } else if ($(window).scrollTop() <= top_start) {

            $('.header').removeClass('pin');
        }
    });
});

// gallery
var swiperSmallThumb = new Swiper(".thumb-small", {
    spaceBetween: 15,
    slidesPerView: 4,
    freeMode: true,
    watchSlidesProgress: true,
    navigation: {
        nextEl: '.thumb-small .swiper-button-next',
        prevEl: '.thumb-small .swiper-button-prev',
    },
});
var swiperBigThumb = new Swiper(".thumb-big", {
    spaceBetween: 15,
    navigation: false,
    thumbs: {
        swiper: swiperSmallThumb,
    },
});

// custom select

$('select').each(function () {
    var $this = $(this), numberOfOptions = $(this).children('option').length;

    $this.addClass('select-hidden');
    $this.wrap('<div class="select"></div>');
    $this.after('<div class="select-styled"></div>');

    var $styledSelect = $this.next('div.select-styled');
    $styledSelect.text($this.children('option').eq(0).text());

    var $list = $('<ul />', {
        'class': 'select-options'
    }).insertAfter($styledSelect);

    for (var i = 0; i < numberOfOptions; i++) {
        $('<li />', {
            text: $this.children('option').eq(i).text(),
            rel: $this.children('option').eq(i).val()
        }).appendTo($list);
        if ($this.children('option').eq(i).is(':selected')) {
            $('li[rel="' + $this.children('option').eq(i).val() + '"]').addClass('is-selected')
        }
    }

    var $listItems = $list.children('li');

    $styledSelect.click(function (e) {
        e.stopPropagation();
        $('div.select-styled.active').not(this).each(function () {
            $(this).removeClass('active').next('ul.select-options').hide();
        });
        $(this).toggleClass('active').next('ul.select-options').toggle();
    });

    $listItems.click(function (e) {
        e.stopPropagation();
        $styledSelect.text($(this).text()).removeClass('active');
        $this.val($(this).attr('rel'));
        $list.find('li.is-selected').removeClass('is-selected');
        $list.find('li[rel="' + $(this).attr('rel') + '"]').addClass('is-selected');
        $list.hide();
    });

    $(document).click(function () {
        $styledSelect.removeClass('active');
        $list.hide();
    });
});

$(function () {
    $("#datepicker").datepicker();
});
function scrollTop() {
    if ($(window).scrollTop() > 500) {
        $(".backToTopBtn").addClass("active");
    } else {
        $(".backToTopBtn").removeClass("active");
    }
}
$(function () {
    scrollTop();
    $(window).on("scroll", scrollTop);

    $(".backToTopBtn").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1);
        return false;
    });
});

// hỗ trợ
$(document).ready(function () {
    $(".list-faq-v2 .item > .title-faq").on("click", function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
            $(this)
                .siblings(".answer")
                .slideUp(300);
        } else {
            $(".list-faq-v2 .item > .title-faq").removeClass("active");
            $(this).addClass("active");
            $(".answer").slideUp(300);
            $(this)
                .siblings(".answer")
                .slideDown(300);
        }
    });
});
