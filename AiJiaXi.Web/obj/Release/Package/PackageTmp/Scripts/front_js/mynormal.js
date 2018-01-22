//全局点击对象
var clickObj;

//移动端解决300ms延迟 fastclick插件方法
$(function () { FastClick.attach(document.body); });

function ajaxLocal(urlPage, data, success) {
    showLoading();
    return $.ajax({
        url: urlPage,
        dataType: 'json',
        //contentType: "application/x-www-form-urlencoded;charset=utf-8",
        type: 'POST',
        data: data,
        async: true,
        timeout: 30000,
        success: success,
        complete: function (data) {
            //alert(data);
            hideLoading();
        },
        error: function () {
            //alert("error");
        },
    });
}

function Msg(sContent, sType) {// type 1:成功 2:提醒 3:错误
    $('#info').remove();
    var sTime = 3000;
    var tips_box = '';
    var loadIngBox = '<div id="loadBg"></div>';
    var loadBg = '<div class="bg"></div>';
    var tips_info = '<div id="info"></div>';
    var tips_type;
    var randId = Math.ceil(Math.random() * 100000);
    var objId = 'tipBox_' + randId;
    switch (sType) {
        case 1:
            tips_type = '<i class="success"></i>';
            break;
        case 2:
            tips_type = '<i class="warn"></i>';
            break;
        case 3:
            tips_type = '<i class="error"></i>';
            break;
        default:
            tips_type = '<i class="success"></i>';
            break;
    }

    //$('body').append(loadIngBox);
    //$('#loadBg').append(loadBg);
    $('body').append(tips_info);
    $('#info').html(tips_type + sContent);
    $('#info').css({ left: parseInt(($(window).width() - $('#info').width() - 52) / 2), top: parseInt(($(window).height() - $('#info').height() - 48) / 2) });

    setTimeout(function () { $("#info").remove() }, sTime);
}

function checkTel(tel) {
    var reg = /^0?1[3|4|5|7|8][0-9]\d{8}$/;
    if (reg.test(tel)) {
        return true;
    } else {
        return false;
    }
}

var loadingTimer;

//加载loadIng背景
function showLoading() {
    hideLoading();
    loadingTimer = setTimeout(function () {
        var loadIngBox = '<div id="loaderWarp"><div class="bg"></div><div class="ball-triangle-path"><div></div><div></div><div></div></div></div>';
        $('body').append(loadIngBox);
        $('#loaderWarp .ball-triangle-path').css("top", $(window).height() / 2 + 'px');
        $('#loaderWarp .ball-triangle-path').css("left", $(window).width() / 2 + 'px');
    }, 1000);
}

function hideLoading() {
    clearTimeout(loadingTimer);
    $('#loaderWarp').remove();
}

bindClick($('header span.back'), function () { window.history.go(-1); });
bindClick($('.pageTurn'), function () { pageTurn($(this).attr('data-url')); });

$('header.reback span.back').unbind('touchstart');//清除副页back事件
$('header.reback span.back').unbind('click');//清除副页back事件

/*document.getElementsByTagName('body')[0].addEventListener('touchmove',function(event){
    event.preventDefault();//阻止浏览器的默认事件
});*/

$(document).bind("selectstart", function () { return false; });//禁止选择文本

if (store('pageName') != 'main') {
    var homeIcon = '<i class="icon home"></i>';
    $('section header').append(homeIcon);
    bindClick($('section header i.home'), function () {
        pageTurn('/baixime/Index/' + store('PcityName') + '/' + store('cityName') + '/2/' + store('OpenId') + '/');
       // pageTurn('/frontpage/index');
    });
}

//if(store('pageName')=='address_list'){
//$('body').on("touchmove",(function(){event.preventDefault();}));
//}

function bindClick(obj, func) {
    obj.unbind();
    //obj.on("touchstart",(func));
    obj.on("click", (func));
}

function pageTurn(url) { window.location.href = url; }

function putAjaxData(arr) {
    var Data = new Array();
    for (key in arr) {
     
        Data = Data.concat('{id:' + '"' + key + '",count:"' + arr[key] + '"}');
    }
    Data = '[' + Data.join(',') + ']';
    Data = eval('(' + Data + ')'); //转化成json格式
    ajaxData = new Array();
    return Data;
}

function MsgHead(str, type, time) {
    if (!time) time = 5000;
    var mClass = 'success';
    switch (type) {
        case 1:
            mClass = 'success';
            break;
        case 2:
            mClass = 'warning';
            break;
        case 3:
            mClass = 'normal';
            break;
    }
    var headMsg = '<div class="msgHead ' + mClass + '">' + str + '</div>';
    $('body section header').after(headMsg);
    setTimeout(function () {
        $('div.msgHead').css('height', 0);
        $('div.msgHead').css('margin-bottom', 0);
    }, time);

    setTimeout(function () {
        $('body section header .msgHead').remove();
    }, time + 1200);
}

bindClick($('div.callTel'), function () {
    var phone = "4000901082";
    location.href = "tel:" + phone;
});

function transform(obj, style) {
    obj.css('transform', 'translateX(' + style + ')');
    //obj.css('-webkit-transform','translateX('+style+')');
};

$('.flashUp').css('transform', 'translateY(60px)');
$('.flashDown').css('transform', 'translateY(-60px)');
$('.flashLeft').css('transform', 'translateX(60px)');

$(function () {
    $('.flashUp').each(function () {
        var obj = $(this);
        var step = obj.attr('flash-step');
        var time = step ? step : 100;
        setTimeout(function () {
            obj.addClass('flashShow');
            obj.css('transform', 'translateY(0)');
        }, time);
    });

    $('.flashDown').each(function () {
        var obj = $(this);
        var step = obj.attr('flash-step');
        var time = step ? step : 100;
        setTimeout(function () {
            obj.addClass('flashShow');
            obj.css('transform', 'translateY(0)');
        }, time);
    });

    $('.flashLeft').each(function () {
        var obj = $(this);
        var step = obj.attr('flash-step');
        var time = step ? step : 100;
        setTimeout(function () {
            obj.addClass('flashShow');
            obj.css('transform', 'translateX(-80px)');
        }, time);
        setTimeout(function () {
            obj.css('transform', 'translateX(0px)');
        }, parseInt(time) + 300);
    });
});

$('.flashStep').css('transform', 'translate(40px,20px)');

function flashStart() {
    setTimeout(function () {
        $('.flashStep').addClass('flashShow');
        $('.flashStep').css('transform', 'translate(0,0)');
    }, 100);
}

function flashReset() {
    $('.flashStep').removeClass('flashShow');
    $('.flashStep').css('transform', 'translate(40px,20px)');
}

function flashLeftStart() {
    $('.flashLeftStep').each(function () {
        var obj = $(this);
        var step = obj.attr('flash-step');
        var time = step ? step : 100;
        setTimeout(function () {
            obj.addClass('flashShow');
            obj.css('transform', 'translateX(-40px)');
        }, time);
        setTimeout(function () {
            obj.css('transform', 'translateX(0px)');
        }, parseInt(time) + 300);
    });
}

function flashLeftReset() {
    $('.flashLeftStep').removeClass('flashShow');
}

function is_weixin() {

    return false;
}

if (typeof (openInWechat) == "undefined") {
    is_weixin();
}

/*
store(key, data);                 //单个存储字符串数据
store({key: data, key2: data2});  //批量存储多个字符串数据
store(key);                       //获取key的字符串数据
store();                          //获取所有key/data
//store(false);（弃用）            //因为传入空值 或者报错很容易清空库
store(key,false);                 //删除key包括key的字符串数据

store.set(key, data[, overwrite]);//=== store(key, data);
store.setAll(data[, overwrite]);  //=== store({key: data, key2: data});
store.get(key[, alt]);            //=== store(key);
store.getAll();                   //=== store();
store.remove(key);                //===store(key,false)
store.clear();                    //清空所有key/data
store.keys();                     //返回所有key的数组
store.forEach(callback);          //循环遍历，返回false结束遍历

store.has(key);         //⇒判断是否存在返回true/false        
*/