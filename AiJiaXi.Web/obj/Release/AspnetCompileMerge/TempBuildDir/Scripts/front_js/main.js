var ItemId = '';
var MoveBeginX = 0;
var MoveBeginY = 0;
var MenuNum = 0;
var LockResetItemNum = 0;
var msgTimeLocked = 0;
var itemAllNum = parseInt($('#all_num').html());
var numTimeout;
var numTimeout2;
var numTimeout3;
var ProductJsonTemp;//之前加载的缓存
//var LoginOpen = 0;

var Main = {

    defaultClick: function () {

        /*** 活动单页居中判断 ***/
        var boxHeight = $(window).height() - 98 - 50;
        var imgHeight = $('div.category img.hot').height();
        //$('div.category img.hot').css('margin-top',60);
        $('#loginWarp h2').css('top', boxHeight + 80);
        $('#loginWarp h4').css('top', boxHeight + 120);


        //alert(store('ProductJson'));
        //将上次的缓存存到变量中
        if (store('ProductJson') != undefined && store('ProductJson') != '') {
            if (store('ProductJson').length > 0) {
                ProductJsonTemp = store('ProductJson');

                //循环触发发点击添加小红点按钮
                for (var i = 0; i < ProductJsonTemp.length; i++) {
                    //找到该商品选择了几次
                    var cou = ProductJsonTemp[i].count;
                
                    //触发实现该商品几次
                    for (var j = 0; j < cou; j++) {
                        var x = $('i.numWarp s.add').clientX;
                        var y = $('i.numWarp s.add').clientY;
                        //添加小红点
                        Main.itemAddDot(x, y);
                        //$(this).parent().parent().parent().parent();
                        ItemId = ProductJsonTemp[i].id.replace('pro_', '');
                        Main.ResetItemNum('add');
                    }



                }
            }

        }

        /*** 判断购物车数量 ***/
        if (parseInt($('#all_num').html()) > 0) {
            $('#all_num').show();
        }


        /*** 菜单宽度计算 ***/
        MenuNum = $('nav.main span').size() - 1;
        $('nav.main span').css('width', 25 + "%");
        $('section.main div.categoryWarp').css('width', 100 * MenuNum + "%");
        $('section.main div.categoryWarp div.category').css('width', 100 / MenuNum + "%");

        setTimeout(function () {
            $('nav.main').show();
            $('section.main div.categoryWarp').show();
        }, 10);

        /*** 如果有菜单并且没有活动则默认切换第一项 ***/
        if (MenuNum > 1 && !hasActivity) {
            //alert(1);
            var tempIndex=0;//LC增加，暂时代表第0个分类
            $('nav span i').addClass("g");
            $('nav span').eq(tempIndex).find('i').removeClass("g");
            var index =tempIndex;
            var move = index * 100;
            transform($('nav span.line'), move + '%');
            transform($('div.categoryWarp'), (-move / MenuNum) + '%');

            //$('section.main div.category').css('height','auto');
            setTimeout(function () {
                //$('div.categoryWarp').css('height',boxHeight+50);
                $('section.main div.category').css('height', 'auto');
                var menuPageHeight = $('div.categoryWarp div.category').eq(tempIndex).height();
                $('div.categoryWarp').css('height', menuPageHeight);
                $('section.main div.category').css('height', '100%');
            }, 1000);
            //$('section.main div.category').css('height','100%');


        };

        /*** 菜单翻转 ***/
        bindClick($('nav span'), function () {
           
            if ($(this).find('i').hasClass('g')) {
                //if($(this).find('i').css("background-image").length>0){
                $('body').scrollTop(0);
                $('nav span i').addClass("g");
                $(this).find('i').removeClass("g");

                for (var i = 0; i < $('nav span').length; i++) {
                    var dbg = $('nav span').eq(i).find('i').data("bg");
                    $('nav span').eq(i).find('i').css("background-image", "url('" + dbg + "')");
                }
                var abg = $(this).find('i').data("img");
                $(this).find('i').css("background-image", "url('" + abg + "')");

                var index = $(this).index();
                var mymove;
                if ($(this).index() > 3) {
                    mymove = (index - 4) * 100;
                    $('nav span.line').css("margin-top", "50px");
                } else {
                    mymove = index * 100;
                    $('nav span.line').css("margin-top", "0px");
                }
                var move = index * 100;

                $('div.categoryWarp div.category .flashLeft').each(function () {
                    var obj = $(this);
                    var step = obj.attr('flash-step');
                    var time = (600 - step) / 2;
                    obj.removeClass('flashShow');
                    setTimeout(function () {
                        obj.css('transform', 'translateX(160px)');
                    }, time);
                });

                $('div.categoryWarp div.category').eq(index).find('.flashLeft').each(function () {
                    var obj = $(this);
                    var step = obj.attr('flash-step');
                    var time = parseInt(step) + 200;
                    setTimeout(function () {
                        obj.css('transform', 'translateX(0)');
                        obj.addClass('flashShow');
                    }, time);
                });





                setTimeout(function () {
                    transform($('nav span.line'), mymove + '%');
                    transform($('div.categoryWarp'), (-move / MenuNum) + '%');
                }, 400);


                $('section.main div.category').css('height', 'auto');
                var menuPageHeight = $('div.categoryWarp div.category').eq(index).height();
                $('div.categoryWarp').css('height', menuPageHeight);
                $('section.main div.category').css('height', '100%');
            }
        });

        /*** 跳转地址页 ***/
        bindClick($('header.main span.left'), function () {
            $('section.city').show();
            $('#cart').hide();
            setTimeout(function () {
                transform($('section.city'), '-100%');
                transform($('section.main'), '-100%');
                transform($('header.main'), '-100%');
                transform($('nav.main'), '-100%');
            }, 10);
            setTimeout(function () {
                $('section.main').hide();
                $('header.main').hide();
                $('nav.main').hide();
            }, 320);
        });

        /*** 返回主页 ***/
        bindClick($('section.city header.reback span.left'), function () {

            $('section.main').show();
            $('header.main').show();
            $('nav.main').show();
            $('#cart').show();
            setTimeout(function () {
                transform($('section.city'), '0');
                transform($('section.main'), '0');
                transform($('header.main'), '0');
                transform($('nav.main'), '0');
            }, 10);
            setTimeout(function () { $('section.city').hide(); }, 320);
        });

        /*** 选择地址 ***/
        bindClick($('section.city ul.open li'), function () {
            if (!$(this).find('i').hasClass('sel')) {

                var CountyId = $(this).attr('data-id'); //区域ID
                var CityId = $(this).attr('id'); //城市ID
                Main.turnChangeCity(CityId, CountyId);
                // pageTurn('/baixime/Index/' + CityId + '/' + CountyId + '/');
            }
        });

        /*** 点击添加/减少商品 ***/
        bindClick($('i.numWarp s.add'), function (e) {
            var x = e.clientX;
            var y = e.clientY;
            Main.itemAddDot(x, y);
            //$(this).parent().parent().parent().parent();
            ItemId = $(this).parent().attr('data-value');
            Main.ResetItemNum('add');
        });

        bindClick($('i.numWarp s.del'), function () {
            ItemId = $(this).parent().attr('data-value');
            Main.ResetItemNum('del');
        });

        /*** 输入框自适应宽度 ***/
        /*setTimeout(function(){
			var login_p_width = parseInt($(window).width()*85/100);
			$('#login_tel').css('width',login_p_width-142);
			$(window).resize(function(){
				var login_p_width = $('#login_tel').parent().width();
				$('#login_tel').css('width',login_p_width-90);
			});
		},100);*/

        /*** 空间和购物车点击事件绑定 ***/

        if (store('login')) {
            bindClick($('i.space'), function () {

                // alert('FrontPage/UserInfo?openId=' + store('OpenId') + '&cityName=' + store('PcityName') + '&countyName=' + store('cityName') + '');
                pageTurn('/FrontPage/UserInfo?openId=' + store('OpenId') + '&cityName=' + store('PcityName') + '&countyName=' + store('cityName') + '');
                // pageTurn('/space');
            });

            bindClick($('#cart'), function () {
                if (parseInt($('#all_num').html()) > 0) {

                    //循环把商品数量为0的过滤
                    for (var i = 0; i < ProductJson.length; i++) {
                        //找到为0的，删除起始下标，长度为1的一个值
                        var cou = ProductJson[i].count;
                        if (cou == 0)
                        {
                            ProductJson.splice(i, 1);
                        }
                    }
                    var OpenId = store('OpenId');
                    var CountyId = store('cityId');
                    ajaxLocal('/ajax/setCartProduct', { Shopping: ProductJson, Count: ProductJson.length, OpenId: OpenId, CountyId: CountyId }, function (json) {
                        console.log(json);
                        if (json.result) {
                            pageTurn('/Baixime/GenerateOrder?id=' + json.id + '&countyId=' + CountyId);
                        } else {
                            if (json.IsFrozen) {
                            Msg('您的帐号已被冻结，不可操作，请联系商家。', 2);
                                return;

                            }
                            //if (json.code == 10) {//用户不存在
                            //    ajaxLocal('/ajax/loginOut', '', function (json) {
                            //        location.reload();
                            //    });
                            //}
                            //Msg(json.msg, 2);
                        }
                    });
                } else {
                    //Msg('');
                }
            });
        } else {
            bindClick($('i.space'), function () {
                $('#loginWarp').show();
                transform($('section.main'), '-85%');
                transform($('header.main'), '-85%');
                transform($('nav.main'), '-85%');
                transform($('#loginWarp'), '-100%');
                $('section.main').css('height', $('#loginWarp').height());
                $('#cart').hide();
                $('section.main').click(function () {
                    transform($('section.main'), '0');
                    transform($('header.main'), '0');
                    transform($('nav.main'), '0');
                    $('section.main').css('height', 'auto');
                    setTimeout(function () { transform($('#loginWarp'), '0'); }, 200);
                    setTimeout(function () { $('#loginWarp').hide(); }, 400);
                    $('#cart').show();
                    $('section.main').unbind('click');
                });
            });

            bindClick($('#cart'), function () {
                $('#loginWarp').show();
                transform($('section.main'), '-85%');
                transform($('header.main'), '-85%');
                transform($('nav.main'), '-85%');
                transform($('#loginWarp'), '-100%');
                $('section.main').css('height', $('#loginWarp').height());
                $('#cart').hide();
                $('section.main').click(function () {
                    transform($('section.main'), '0');
                    transform($('header.main'), '0');
                    transform($('nav.main'), '0');
                    $('section.main').css('height', 'auto');
                    setTimeout(function () { transform($('#loginWarp'), '0'); }, 200);
                    setTimeout(function () { $('#loginWarp').hide(); }, 400);
                    $('#cart').show();
                    $('section.main').unbind('click');
                });
            });
        }

        /*** 退回登陆插页 ***/
        bindClick($('#loginWarp i.back'), function () {
            transform($('section.main'), '0');
            transform($('header.main'), '0');
            transform($('nav.main'), '0');
            $('section.main').css('height', 'auto');
            setTimeout(function () { transform($('#loginWarp'), '0'); }, 200);
            setTimeout(function () { $('#loginWarp').hide(); }, 400);
            $('#cart').show();
            $('section.main').unbind('click');
        });

        /*** 用户协议勾选 ***/
        bindClick($('b.checkbox'), function () {
            if ($(this).hasClass('sel')) {
                $(this).removeClass('sel');
            } else { $(this).addClass('sel'); }
        });

        /*** 输入框事件 ***/
        //$('#login_tel').on("keyup",(function(){
        $('#login_tel').bind('input', (function () {
            if (msgTimeLocked == 0) {
                var tel = $(this).val();
                $('#login_code').val('');
                $('div.login').attr('style', '');
                //alert(tel);
                if (checkTel(tel)) {
                    $('div.yzm').css('background-color', '#2ba870');
                } else {
                    $('div.yzm').attr('style', '');
                }
            }
        }));

        //$('#login_code').on("keyup",(function(){
        $('#login_code').bind('input', (function () {
            var code = $(this).val();
            var tel = $('#login_tel').val();
            if (code.length == 4 && checkTel(tel)) {
                $('div.login').css('background-color', '#2ba870');
            } else {
                $('div.login').attr('style', '');
            }
        }));

        /*** 提交登陆信息 ***/
        bindClick($('#submit'), function () {
      
        
            if ($(this).attr("style")) { //按钮变绿

             
                if (!$('div.loginForm b.checkbox').hasClass('sel')) {
                    Msg('请确认用户协议', 2);
                    return;
                }

                //ajaxData['tel'] = $('#login_tel').val();
                //ajaxData['code'] = $('#login_code').val();
                //ajaxData['registerType'] = 'mobileweb';
                //var data = putAjaxData(ajaxData);




                var Phone = $('#login_tel').val();//手机号
                var code = $('#login_code').val();//验证码

       

                if (store('code') != '' && code != '' && store('code') == code) {
                  
                }
else
{
  Msg('验证码错误，请仔细检查！');
      return;
}
               
                store('code', '');//清除验证码缓存

                //ajaxLocal('/ajax/register', data, function (json) {
                ajaxLocal('/FrontPage/AddUser?OpenId=' + store('OpenId') + '&Phone=' + Phone, null, function (json) {

                    if (json.state == '0') {  //成功
                        //store('userId', json.userId);
                        //store('userName', json.userName);
                        //store('cityId', json.cityId);//需要在后台返回当前区域的ID

                        if (json.IsFrozen)//冻结
                        {
                            Msg(json.msg, 2);
                            return;
                        }

                        store('UserId', json.UserId);//用户表主键ID
                        store('OpenId', json.OpenId);//微信ID
                        store('PhoneNumber', json.PhoneNumber);//手机号
                        store('login', '1');//设置登录成功
                        Msg('登陆成功');
                        // history.go(-1);
                        location.replace('/baixime/Index/' + store('PcityId') + '/' + store('cityId') + '/1/' + store('OpenId') + '/');

                        //$('#login_tel').val('');//手机号
                        //$('#login_code').val('');//验证码

                        //$('#loginWarp').hide();

                        //  pageTurn('/');
                    } else if (json.state == '1') {
                        Msg('帐号创建失败！');
                    }
                    else if (json.state == '999') {
                        Msg('该微信号已经注册过，或该手机号已经和其他微信进行绑定！');
                    }
                    else {
                        Msg('帐号创建失败！', 2);
                    }
                });
            } else {
                return false;
            }
        });

        /*** 获取短信验证码 ***/
        bindClick($('#yzm'), function () {
            if ($(this).attr("style")) { //按钮变绿
                ajaxData['tel'] = $('#login_tel').val();
                if (checkTel(ajaxData['tel'])) {
                    // var data = putAjaxData(ajaxData);
                    //ajaxLocal('/ajax/getMobileCode', data, function (json) {
                    var url = "/FrontPage/GetCode?mobile=" + $('#login_tel').val() + "&token=baiximo@123456";
                    ajaxLocal(url, null, function (json) {

                        //if (json.result == 'Success') {
                        if (json.state == '0')//成功
                        {
                            Msg('短信发送成功');
                            $('#yzm').css('background-color', '');
                            $('#yzm').html(60);
                            setTimeout(Main.timeClick, 1000);
                            msgTimeLocked = 1;

                            store('code', json.code);//验证码加入缓存

                        } else {
                          
                            Msg(json.msg, 2);
                        }
                    });
                }
            } else {
                return false;
            }
        });

        /*** 切换用户协议 ***/
        bindClick($('#loginWarp span.xieyi a'), function () {
            $('section.main_news').show();
            setTimeout(function () {
                transform($('section.main'), '-100%');
                transform($('header.main'), '-100%');
                transform($('nav.main'), '-100%');
                transform($('section.main_news'), '-100%');
                transform($('#loginWarp'), '-220%');
                flashStart();
            }, 10);
            setTimeout(function () {
                $('section.main').hide();
                $('header.main').hide();
                $('nav.main').hide();
            }, 300);
        });

        bindClick($('section.main_news header.reback span.back'), function () {
            $('section.main').show();
            $('header.main').show();
            $('nav.main').show();
            setTimeout(function () {
                transform($('section.main'), '-85%');
                transform($('header.main'), '-85%');
                transform($('nav.main'), '-85%');
                transform($('section.main_news'), '0%');
                transform($('#loginWarp'), '-100%');
                flashReset();
            }, 10);
            setTimeout(function () {
                $('section.main_news').hide();
            }, 300);
        });

        /*** 根据城市ID筛选显示 ***/
        $('section.fanwei box').each(function () {
            var value = $(this).attr('data-value');
            if (value != store('cityId')) { $(this).hide(); }
        });

        /*** 切换城市服务范围 ***/
        bindClick($('#check_fw'), function () {
            $('section.fanwei').show();
            setTimeout(function () {
                transform($('section.fanwei'), '-100%');
                transform($('section.city'), '-200%');
                flashStart();
            }, 10);
            setTimeout(function () {
                $('section.city').hide();
            }, 300);
        });

        /*** 返回城市页 ***/
        bindClick($('section.fanwei header.reback span.left'), function () {
            $('section.city').show();
            setTimeout(function () {
                transform($('section.fanwei'), '0');
                transform($('section.city'), '-100%');
            }, 10);
            setTimeout(function () { $('section.fanwei').hide(); }, 300);
        });

        /*$('section.main div.category div.cate').on("touchstart",(function(e){
			//event.preventDefault();
			MoveBeginX = e.originalEvent.targetTouches[0].pageX;
			$(this).removeClass('flashLeft');
			//$(this).css('-webkit-transform','translateX(-100px)');
		}));

		$('section.main div.category div.cate').on("touchmove",(function(e){
			var x = e.originalEvent.targetTouches[0].pageX;
			var MoveX = x - MoveBeginX;
			$(this).find('h3').html(MoveX);
			$(this).css('-webkit-transform','translateX('+MoveX+'px)');
		}));
		
		$('section.main div.category div.cate').on("touchend",(function(e){
			$(this).addClass('flashLeft');
			$(this).css('-webkit-transform','translateX(0)');
		}));*/
    },

    //添加衣服小红点效果
    itemAddDot: function (x, y) {
        $('.addDot').remove();
        var addDot = "<div class='addDot'></div>";
        $('body').append(addDot);
        $('.addDot').css('left', x);
        $('.addDot').css('top', y);
        setTimeout(function () {
            $('.addDot').addClass('flashShow');
            $('.addDot').css('width', 14);
            $('.addDot').css('height', 14);
            $('.addDot').css('transform', 'translate(-30px,-30px)');
        }, 10);

        var Wheight = $(window).height() - y - 70;
        var Wwidth = $(window).width() - x - 30;

        setTimeout(function () {
            $('.addDot').css('transform', 'translate(' + Wwidth + 'px,' + Wheight + 'px)');
        }, 210);
    },

    timeClick: function () {
        var clickNum = parseInt($('#yzm').html());
        if (clickNum > 0) {
            $('#yzm').html(clickNum - 1);
            setTimeout(Main.timeClick, 1000);
        } else {
            $('#yzm').html("获取验证码");
            var tel = $('#login_tel').val();
            if (checkTel(tel)) {
                $('#yzm').css('background-color', '#2ba870');
            }
            msgTimeLocked = 0;
        }
    },

    ResetItemNum: function (type) {
        if (LockResetItemNum == 0) {
            LockResetItemNum = 1;
            var NumWarp = $('#' + ItemId + '_num');
            var Num = parseInt(NumWarp.html());
            //var NumAll = itemAllNum;
            var ParentWarp = NumWarp.parent();
            var ProductId = ParentWarp.attr('data-id');
            var ItemPrice = parseInt(NumWarp.parent().attr('data-price'));
            if (type == 'add') {
                Num = Num + 1;
                CartPrice += ItemPrice;
                if (Num == 1) {
                    ParentWarp.css('width', 100);
                    //ParentWarp.find('i.add2').css('-webkit-transform','rotate(180deg)');
                }
                if (itemAllNum == 0) { $('#all_num').show(); }
                NumWarp.html(Num);
                clearTimeout(numTimeout);
                clearTimeout(numTimeout2);
                clearTimeout(numTimeout3);
                itemAllNum += 1;
                numTimeout = setTimeout(function () {
                    //$('#all_num').html(itemAllNum);
                    $('#all_num').html('￥' + CartPrice);
                    $('#all_num').css('font-size', '40px');
                    $('.addDot').remove();
                }, 410);
                numTimeout2 = setTimeout(function () {
                    $('#all_num').css('font-size', '18px');
                }, 610);
                numTimeout3 = setTimeout(function () {
                    $('#all_num').html(itemAllNum);
                }, 810);
            } else if (type == 'del') {
                Num = Num - 1 > 0 ? Num - 1 : 0;
                CartPrice -= ItemPrice;
                if (Num == 0) {
                    ParentWarp.css('width', 0);
                    //ParentWarp.find('i.add2').css('-webkit-transform','rotate(0deg)');
                }
                if (itemAllNum == 1) { $('#all_num').hide(); }
                NumWarp.html(Num);
                itemAllNum -= 1;
                $('#all_num').html(itemAllNum);
            }
            ProductCart['pro_' + ProductId] = Num;
            ProductJson = putAjaxData(ProductCart);

            store('ProductJson', ProductJson);
            //ajaxLocal('/ajax/setCartProduct',ProductJson,function(json){
            //	if(json.result=='Success'){
            //		//pageTurn('/order/subcart');
            //	}else{
            //		//Msg(json.msg,2);
            //	}
            //	LockResetItemNum = 0;
            //});
            LockResetItemNum = 0;
        }
    },


    /*** 商品高度自适应 ***/
    resetImg: function () {
        var cateWarpHeight = $(window).height() - 98;
        var cate = $('section.main div.categoryWarp div.category div.cate');
        var cateHeight = cateWarpHeight / 3 + 1;
        cate.css('height', cateHeight);
        cate.find('img').css('height', 'auto');
        cate.find('img').css('width', '100%');
        var imgHeight = cate.eq(0).find('img').height();
        if (imgHeight > cateHeight) {
            //alert(cateHeight-imgHeight);
            cate.find('img').css('margin-left', 0);
            cate.find('img').css('margin-top', (cateHeight - imgHeight) / 2);
        } else {
            if (!imgHeight) {
                setTimeout(Main.resetImg, 100);
            }
            cate.find('img').css('margin-top', 0);
            cate.find('img').css('height', cateHeight);
            cate.find('img').css('width', 'auto');
            var imgWidth = cate.eq(0).find('img').width();
            if (imgWidth > $(window).width()) {
                var widthStep = imgWidth - $(window).width();
                cate.find('img').css('margin-left', -widthStep / 2);
            }
        }
    },


    setImgHuodongClick: function () {
        //活动商品点击事件
        $('img.huodong,span.huodong').click(function () {
            var ProductId = $(this).attr('data-id');
            var HDProductCart = new Array();
            HDProductCart['pro_' + ProductId] = 1;
            var HDProductJson = putAjaxData(HDProductCart);
            ajaxLocal('/ajax/setCartProduct', HDProductJson, function (json) {
                if (json.result == 'Success') {
                    pageTurn('/order/subcart');
                } else {
                    if (json.result == 'False') { Msg('请先登陆', 2); return; }
                    if (json.code == 10) {//用户不存在
                        ajaxLocal('/ajax/loginOut', '', function (json) {
                            location.reload();
                        });
                    }
                    Msg(json.msg, 2);
                }
            });
        });
    },
    //城市ID和区域ID
    turnChangeCity: function (cityId, CountyId) {

        var obj = $('div.turnShow');
        var areaBox;
        obj.show();

        $('section.fanwei box').each(function () {
            var value = $(this).attr('data-value');
            //var value = $(this).attr('id');
            if (value == CountyId) {

                areaBox = $(this);

            }
        });
        console.log(areaBox.find('span').text());
        obj.find('.t span').html(areaBox.find('span').text());
        obj.find('.c').html(areaBox.find('p').text());
        obj.find('.n').html(areaBox.find('img').eq(1));
        obj.find('.w').click(function () {
            //pageTurn('/baixime/Index/' + cityId + '/' + CountyId + '/1/' + store('OpenId') + '/');

          
            location.replace('/baixime/Index/' + cityId + '/' + CountyId + '/1/' + store('OpenId') + '/');

            $('section.city').hide();
            obj.hide();
            $('section.main').show();
            $('header.main').show();
            $('nav.main').show();
            $('#cart').show();
            setTimeout(function () {
                transform($('section.city'), '0');
                transform($('section.main'), '0');
                transform($('header.main'), '0');
                transform($('nav.main'), '0');
            }, 10);
            setTimeout(function () { $('section.city').hide(); }, 320);
        });
        //Main.timeStep(obj.find('.n'),cityId);
        // pageTurn('/baixime/Index/' + cityId + '/' + CountyId + '/');
    },

    timeStep: function (obj, cityId) {
        var obj = obj;
        var timeInter;
        timeInter = setInterval(function () {
            if (obj.html() == 1) {
                obj.html(obj.html() - 1);
                pageTurn('/baixime/Index/' + cityId + '/');
            } else if (obj.html() <= 0) {
                clearInteval(timeInter);
            } else {
                obj.html(obj.html() - 1);
            }
        }, 1000);
    },

    /**
	* @ 页面初始化
	*/
    init_page: function (type) {
        Main.defaultClick();
        Main.setImgHuodongClick();
        setTimeout(Main.resetImg, 10);
        $(window).resize(function () { Main.resetImg(); });
    }
}