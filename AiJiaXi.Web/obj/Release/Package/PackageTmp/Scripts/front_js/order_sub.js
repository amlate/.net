if (typeof (DYSDETAIL) == "undefined") {
    var Order = new Object();    // 创建登录页面命名空间
}

var ItemId = '';
var MoveSection = '';
var MoveBeginY = 0;
var MoveContentY = 0;
var MoveContentH = 0;
var Product = new Array();
var CouponLi;

var Order = {

    defaultClick: function () {

        $('section.order_sub div.date_sel ul.fr li').each(function () {
            if ($(this).index() < serverHour) {
                $(this).hide();
            } else if ($(this).index() == serverHour) {
                $(this).addClass('sel');
            }
        });

        $('div.content').css('height', $(window).height() - 88);
        $('#order_submit').css('width', $(window).width() - 170);
        $(window).resize(function () {
            $('div.content').css('height', $(window).height() - 88);
            $('#order_submit').css('width', $(window).width() - 170);
        });

        //返回上一页
        bindClick($('section.order_sub header span'), function () {

            WeixinJSBridge.call('closeWindow');
            //history.go(-1);
            //pageTurn('/');
        });

        //点击添加小按钮
        bindClick($('section.order_sub b.add'), function () {
            ItemId = $(this).attr('data-value');
     
            //将商品数量增加一个
            var proJson = store('ProductJson');
            if (proJson != undefined && proJson != '') {
                if (proJson.length > 0) {

                    var kk = 0;
                    $.each(proJson, function (index, domEle) {
                        if (ItemId.replace('product_', '') == domEle.id.replace('pro_', ''))
                       {
                            domEle.count = parseInt(domEle.count)+1;
                            proJson.splice(index, 1, domEle)
                           
                        }
                    });
                    store('ProductJson', proJson);

                }

            }


            Order.ResetItemNum('add');
        });

        bindClick($('section.order_sub b.del'), function () {
            ItemId = $(this).attr('data-value');
            //将商品数量减少一个
            var proJson = store('ProductJson');
            if (proJson != undefined && proJson != '') {
                if (proJson.length > 0) {

                    var kk = 0;
                    $.each(proJson, function (index, domEle) {
                        if (ItemId.replace('product_', '') == domEle.id.replace('pro_', '')) {
                            domEle.count = parseInt(domEle.count) - 1;
                            proJson.splice(index, 1, domEle)

                        }
                    });
                    store('ProductJson', proJson);

                }

            }

            Order.ResetItemNum('del');
        });

        bindClick($('section.order_cart b.add'), function () {
            ItemId = $(this).attr('data-value');
            Order.ResetItemNum('add');
        });

        bindClick($('section.order_cart b.del'), function () {
            ItemId = $(this).attr('data-value');
            Order.ResetItemNum('del');
        });

        bindClick($('#order_add'), function () {
            Order.TurnPage('order_add');
        });

        bindClick($('#order_cart'), function () {
            Order.TurnPage('order_cart');
        });

        if (parseInt($('#coupon_num').html()) > 0) {
            bindClick($('#order_coupon'), function () {
                Order.TurnPage('order_coupon');
            });
        } else {
            $('#coupon_msg').html('');
        }

        bindClick($('section.order_coupon li'), function () {
            if (!$(this).find('b').eq(1).hasClass('sel')) {
                CouponLi = $(this);

                ajaxData['couponId'] = $(this).attr('data-couponId');
                //alert(ajaxData['couponId']);
                var data = putAjaxData(ajaxData);

                ajaxLocal('/ajax/prepayCart', data, function (json) {
                    if (json.result == 'Success') {
                        Order.resetOrderInfo();
                        Order.BackPage('order_coupon');
                        CouponLi.parent().find('b').removeClass('sel');
                        CouponLi.find('b').addClass('sel');
                    } else {
                        Msg(json.msg, 2);
                    }
                });
            } else {
                Order.BackPage('order_coupon');
            }
        });

        bindClick($('header.reback span.back'), function () {
            $('section.order_sub').show();
            MoveSection = $(this).parent().parent();
            setTimeout(function () {
                transform($('section.order_sub'), '0');
                transform(MoveSection, '0');
            }, 10);
            setTimeout(function () {
                MoveSection.hide();
            }, 420);

        });

        $('section.order_cart ul:last').css('margin-bottom', 60);
        bindClick($('footer.cart_btn'), function () {
            $('section.order_sub').show();
            MoveSection = $(this).parent();
            setTimeout(function () {
                transform($('section.order_sub'), '0');
                transform(MoveSection, '0');
            }, 10);
            setTimeout(function () {
                MoveSection.hide();
            }, 420);
        });

        bindClick($('ul.add_list li'), function () {
            $('ul.add_list li i').removeClass('sel');
            $(this).find('i').addClass('sel');
            $('#deliveryId').val($(this).attr('data-id'));
            var p1 = '<p>' + $(this).attr('data-Contact') + '</p>';
            var p2 = '<p>' + $(this).attr('data-note') + '</p>';
            $('#order_add span').html(p1 + p2);
            $('#order_add i').removeClass('add');
            $('#order_add i').addClass('lt');
            Order.BackPage('order_add');
            
            var service = $(this).attr('data-service');
            if (service != 1) {
                //   MsgHead('服务暂仅开通外环以内，如有不便敬请谅解！', 3);
            }
        });

        bindClick($('ul.add_add li.def'), function () {
            if ($(this).find('span').hasClass('sel')) {
                $('#isDefault').val(0);
                $(this).find('span').removeClass('sel');
            } else {
                $('#isDefault').val(1);
                $(this).find('span').addClass('sel');
            }
        });

        bindClick($('#address_submit'), function () {
            var type = $(this).attr('data-value');
            if (type == 'close') {
                $('section.order_add ul.add_add').css('height', 244);
                $(this).html('确 认 添 加');
                $(this).attr('data-value', 'open');
            } else {
                if (!$('#realname').val()) { Msg('请输入联系人姓名', 2); return; }
                if (!$('#phone').val()) { Msg('请输入手机号', 2); return; }
                else if (!checkTel($('#phone').val())) { Msg('手机格式错误', 2); return; }
                if (!$('#address').val()) { Msg('请输入详细地址', 2); return; }
                if ($('#address').val().length < 6) { Msg('详细地址不能少于六个字', 2); return; }
                //ajaxData['cityId'] = $('#selCity').val();
                var countyId = $('#selDistrict').val();
                var contactPhoneNum = $('#phone').val();
                var contact = $('#realname').val();
                var addr = $('#address').val();
                var isDefault = $('#isDefault').val();
                //var data = putAjaxData(ajaxData);
                var applicationuserId = store('applicationUserId');
                var data = { contactPhoneNum: contactPhoneNum, contact: contact, addr: addr, isDefault: isDefault, applicationuserId: applicationuserId, countyId: countyId }
                ajaxLocal('/ajax/AddOrEditAddress?type=add', data, function (json) {
                    if (json.Valid) {
                        var addId = json.Address.Id;
                        var addPer = json.Address.Contact;
                        var addInfo = json.Address.Addr;
                        var phoneNum = json.Address.ContactPhoneNum;
                        var html = '<li data-id="' + addId + '" data-Contact="' + addPer + ' ' + phoneNum + '" data-note="' + addInfo + '"  data-contactphonenum="' + phoneNum + '">' + addPer + ' ' + phoneNum + '<br>' + addInfo + '<i class="address sel"><b></b></i></li>';
                        $('section.order_add ul.add_list li i').removeClass('sel');
                        $('section.order_add ul.add_list').append(html);
                        var p1 = '<p>' + addPer + ' ' + phoneNum + '</p>';
                        var p2 = '<p>' + addInfo + '</p>';
                        $('#order_add span').html(p1 + p2);
                        $('#order_add i').removeClass('add');
                        $('#order_add i').addClass('lt');
                        Order.BackPage('order_add');

                        if (json.ServiceOverlays != 1) { //MsgHead('服务暂仅开通外环以内，如有不便敬请谅解！', 3); 
                        }

                        $('section.order_add ul.add_add').css('height', 0);
                        $('#address_submit').html('添 加 新 地 址');
                        $('#address_submit').attr('data-value', 'close');
                        $('#deliveryId').val(addId);
                        bindClick($('ul.add_list li'), function () {
                            //$('ul.add_list li i').removeClass('sel');
                            //$(this).find('i').addClass('sel');
                            //$('#addressId').val($(this).attr('data-id'));
                            //var p1 = '<p>' + $(this).attr('data-Contact') + '</p>';
                            //var p2 = '<p>' + $(this).attr('data-note') + '</p>';
                            //$('#order_add span').html(p1 + p2);
                            //$('#order_add i').removeClass('add');
                            //$('#order_add i').addClass('lt');
                            //Order.BackPage('order_add');

                            //var service = $(this).attr('data-service');
                            //if (service != 1) {
                            //  //  MsgHead('服务暂仅开通外环以内，如有不便敬请谅解！', 3);
                            //}
                            $('ul.add_list li i').removeClass('sel');
                            $(this).find('i').addClass('sel');
                            $('#deliveryId').val($(this).attr('data-id'));
                            var p1 = '<p>' + $(this).attr('data-Contact') + '</p>';
                            var p2 = '<p>' + $(this).attr('data-note') + '</p>';
                            $('#order_add span').html(p1 + p2);
                            $('#order_add i').removeClass('add');
                            $('#order_add i').addClass('lt');
                            Order.BackPage('order_add');

                            var service = $(this).attr('data-service');
                        });
                        $('#form_add_add')[0].reset();
                    } else {
                        Msg(json.msg, 2);
                    }
                });
            }
        });

        $('#selCity').on('change', (function () {
            ajaxData['cityId'] = $(this).val();
            if ($(this).val() != store('cityId')) { alert('所选城市跟当前所在城市不同,如需切换城市请返回首页'); }
            var data = putAjaxData(ajaxData);

            ajaxLocal('/ajax/geo/district', data, function (json) {
                if (json.result == 'Success') {
                    $('#selDistrict').html(json.option);
                    if (districtId) $('#selDistrict').val(districtId);
                } else {

                }
            });
        }));

        bindClick($('#order_time ul.fl li'), function () {
            $(this).parent().find('li').removeClass('sel');
            $(this).addClass('sel');

            if ($(this).index() == 0) {
                $('section.order_sub div.date_sel ul.fr li').each(function () {
                    if ($(this).index() < serverHour) {
                        $(this).hide();
                    } else if ($(this).index() == serverHour) {
                        $(this).addClass('sel');
                    }
                });
            } else {
                $('section.order_sub div.date_sel ul.fr li').show();
                $('section.order_sub div.date_sel ul.fr li').removeClass('sel');
                $('section.order_sub div.date_sel ul.fr li').eq(0).addClass('sel');
            }

        });

        bindClick($('#order_time ul.fr li'), function () {
            $(this).parent().find('li').removeClass('sel');
            $(this).addClass('sel');

            $('#order_time p.time_sel').hide();
            var dateInfo = $('#order_time div.date_sel ul.fl li.sel').html() + ' ' + $('#order_time div.date_sel ul.fr li.sel').html();
            $('#pickupDate').val($('#order_time div.date_sel ul.fl li.sel').attr('data-value'));
            $('#pickupTime').val($('#order_time div.date_sel ul.fr li.sel').attr('data-value'));
            $('#order_time p.time_info span').html(dateInfo);
            $('#order_time p.time_info').show();
            $('#order_time div.date_sel').css('height', 0);
        });

        bindClick($('#order_time p.time_none'), function () {
            $(this).hide();
            $('#order_time p.time_sel').show();
            $('#order_time div.date_sel').css('height', 350);
        });

        bindClick($('#order_time p.time_sel'), function () {
            $(this).hide();
            var dateInfo = $('#order_time div.date_sel ul.fl li.sel').html() + ' ' + $('#order_time div.date_sel ul.fr li.sel').html();
            $('#pickupDate').val($('#order_time div.date_sel ul.fl li.sel').attr('data-value'));
            $('#pickupTime').val($('#order_time div.date_sel ul.fr li.sel').attr('data-value'));
            $('#order_time p.time_info span').html(dateInfo);
            $('#order_time p.time_info').show();
            $('#order_time div.date_sel').css('height', 0);
        });

        bindClick($('#order_time p.time_info'), function () {
            $(this).hide();
            $('#order_time p.time_sel').show();
            $('#order_time div.date_sel').css('height', 350);
        });

        bindClick($('footer.order_sub span'), function () {
            if ($(this).attr('data-value') == 'close') {
                $('ul.order_price').show();
                $(this).find('i').css('transform', 'rotate(180deg)');
                setTimeout(function () {
                    $('ul.order_price').css('transform', 'translateY(-130px)');
                }, 10);
                $(this).attr('data-value', 'open');
                $('#subWarp').css('padding-bottom', 112);
            } else {
                $(this).find('i').css('transform', 'rotate(0deg)');
                $('ul.order_price').css('transform', 'translateY(0)');
                setTimeout(function () {
                    $('ul.order_price').hide();
                }, 420);
                $(this).attr('data-value', 'close');
                $('#subWarp').css('padding-bottom', 0);
            }
        });

        $('footer.order_sub span').click();

        bindClick($('#order_submit'), function () {

            ajaxData['UserAddressId'] = $('#deliveryId').val();

            if (!$('#deliveryId').val()) {
                Msg('请添加地址信息', 2); return;
            }

            if ($('#pickupDate').val()) {
                ajaxData['Appointment'] = $('#pickupDate').val();
            } else {
                Msg('请选择预约上门时间', 2); return;
            }
            if ($('#pickupTime').val()) {
                ajaxData['AppointmentTime'] = $('#pickupTime').val();
            } else {
                Msg('请选择预约上门时间', 2); return;
            }
            ajaxData["OrderId"] = $(this).data("value");

            //var data = putAjaxData(ajaxData);
            var Data = new Array();
            for (key in ajaxData) {
                Data = Data.concat(key + ':"' + ajaxData[key] + '"');
            }
            Data = '{' + Data.join(',') + '}';
            Data = eval('(' + Data + ')'); //转化成json格式
            ajaxLocal('/Ajax/OrderConfirm', Data, function (json) {
                if (json.result) {
                    //清空购物车缓存
                    store('ProductJson', '');
                    // store('itemAllNum', '');


                    pageTurn('/FrontPage/orderlist?openid=' + store('OpenId') + '&type=0&orderid=' + json.OrderNo + '&countyId=' + store('countyId'));
                    // pageTurn('/FrontPage/Pay/' + Data.OrderId);                    
                } else {
                    Msg(json.msg, 2);
                }
            });
        });


    },

    ResetItemNum: function (type) {
        var objLiWarp = $('#' + ItemId);
        var objNumWarp = $('#' + ItemId + '_num');
        var objLiCartWarp = $('#' + ItemId + '_cart');
        var objNumCartWarp = $('#' + ItemId + '_num_cart');
        var objNum = parseInt(objNumWarp.html());
        if (type == 'add') {
            objNum = objNum + 1;
            if (objNum == 1) {
                objLiWarp.removeClass('none');
                objLiCartWarp.find('span.item_num').removeClass('none');
            }
            objNumWarp.html(objNum);
            objNumCartWarp.html(objNum);
        } else if (type == 'del') {
            objNum = objNum - 1 > 0 ? objNum - 1 : 0;
            if (objNum == 0) {
                objLiWarp.addClass('none');
                objLiCartWarp.find('span.item_num').addClass('none');
            }
            objNumWarp.html(objNum);
            objNumCartWarp.html(objNum);
        }
        var key = ItemId.replace('duct', '');
        Product = new Array();
        Product[key] = objNum;
        var ProductJson = putAjaxData(Product);
        ajaxLocal('/ajax/UpdateCartProduct', { ProductJson: ProductJson, OrderId: objLiWarp.data("value") }, function (json) {
            if (json.result) {
                Order.resetOrderInfo(objLiWarp.data("value"));
            } else {
                //Msg(json.msg, 2);
            }
        });
    },

    TurnPage: function (type) {
        $('section.' + type).show();
        setTimeout(function () {
            transform($('section.' + type), '-100%');
            transform($('section.order_sub'), '-100%');
        }, 10);
        setTimeout(function () {
            $('section.order_sub').hide();
        }, 420);
    },

    BackPage: function (type) {
        $('section.order_sub').show();
        setTimeout(function () {
            transform($('section.' + type), '0');
            transform($('section.order_sub'), '0');
        }, 10);
        setTimeout(function () {
            $('section.' + type).hide();
        }, 420);
    },

    resetOrderInfo: function (OrderId) {
        ajaxLocal('/ajax/GetCartProduct', { OrderId: OrderId }, function (json) {
            if (json.result) {
                $('#cartProductFee').html(json.Total + '元');
                $('#cartDeliveryFee').html('0元');
                $('#cartOrderFee').html(json.Total + '元');
                if (json.couponMsg) { $('#coupon_msg').html(json.couponMsg); }
                if (json.deliveryFee == 0) {
                    //$('ul.order_price li i.r').show();
                } else {
                    //$('ul.order_price li i.r').hide();
                }
                if (json.freeDeliveryMsg) {
                    $('ul.order_price li').eq(1).find('i.r').show();
                    $('ul.order_price li').eq(1).find('i.r').html(json.freeDeliveryMsg);
                } else {
                    $('ul.order_price li').eq(1).find('i.r').hide();
                }
                if (json.freeOtherMsg) {
                    $('ul.order_price li').eq(0).find('i.r').show();
                    $('ul.order_price li').eq(0).find('i.r').html(json.freeOtherMsg);
                } else {
                    $('ul.order_price li').eq(0).find('i.r').hide();
                }
            } else {
                Msg(json.msg, 2);
            }
        });
    },

    /**
	* @ 页面初始化
	*/
    init_page: function (type) {
        Order.defaultClick();
    }
}