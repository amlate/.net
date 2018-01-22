if (typeof (DYSDETAIL) == "undefined") {
    var Address = new Object();    // 创建登录页面命名空间
}

var pageH = 0;
var addWarp; //全局地址列表 单个 LI 对象
var districtId = 0;

var Address = {

	defaultClick:function(){
		
		bindClick($('#add_page'),function(){
			$('header.reback span.back span').html('添加地址');
			$('#address_submit').html('确 认 添 加');
			$('#add_type').val('add');
			Address.printAddBox();
			Address.TurnPage();
		});

		bindClick($('header.reback span.back'),function(){
			Address.BackPage('address_add');
		});

		bindClick($('header.list span.back'),function(){
		    //pageTurn('/space');
            history.go(-1);
		});

		bindClick($('section.address_add ul.add_add li.def'),function(){
			if($(this).find('span').hasClass('sel')){
				$('#isDefault').val(0);
				$(this).find('span').removeClass('sel');
			}else{
				$('#isDefault').val(1);
				$(this).find('span').addClass('sel');
			}
		});

		$('section.address_list ul.add_list li').each(function(){
			//$(this).css('height',$(this).height());
			$(this).find('span').css('line-height',$(this).innerHeight()+"px");
		});

		bindClick($('section.address_list ul.add_list li'),function(){
			transform($('section.address_list ul.add_list li'),'0');
			if($(this).attr('move-data')==1){
				$(this).attr('move-data',0);
			}else{
				//$('section.address_list ul.add_list li span').hide();
				//$(this).find('span').show();
				transform($(this),'-160px');
				$('section.address_list ul.add_list li').attr('move-data',0);
				$(this).attr('move-data',1);
			}
		});

		bindClick($('section.address_list ul.add_list li span.del'),function(){
			addWarp = $(this).parent();
			var confg = confirm("确定要删除这条地址？");
			if(confg==true){
			    var id = addWarp.attr('data-id');
			    var openid = addWarp.attr('data-openid');
			    var data = { id: id, openid: openid };
			    ajaxLocal('/ajax/DelAddress', data, function (json) {
			        if (json.Valid) {
						addWarp.css('height',addWarp.height());
						setTimeout(function(){
							addWarp.css('height',0);
							addWarp.addClass('hide');
						},10);
					}else{
			            Msg(json.Msg, 2);
					}
				});
			}
		});

		bindClick($('section.address_list ul.add_list li span.upd'),function(){
		    addWarp = $(this).parent();
			$('header.reback span.back span').html('编辑地址');
			$('#address_submit').html('确 认 编 辑');
			$('#add_type').val('upd');
			Address.TurnPage(addWarp);
			var id = addWarp.attr("data-id");
			$("#addressId").val(id);
			var contact = addWarp.attr("data-Contact");
			$("#realname").val(contact);
			var contactPhoneNum = addWarp.attr("data-ContactPhoneNum");
			$("#phone").val(contactPhoneNum);
		});

        //LC注释，，城市目前默认为首页左上角的，在AddAddress.cshtml中初始化设置
		//$('#selCity').on('change',(function(){
		//	var cityId= $(this).val();
		//    //if($(this).val()!=store('cityId')){alert('所选城市跟当前所在城市不同,如需切换城市请返回首页');}
		//    var data = {};
		//    ajaxLocal('/ajax/GetCounty?cityid=' + cityId,data,function (json) {
		//	    if (json.Valid) {
		//			$('#selDistrict').html(json.list);
		//		    //if(districtId)$('#selDistrict').val(districtId);
		//		}else{
					
		//		}
		//	});
		//}));

		bindClick($('#address_submit'), function () {
		    var addType = $('#add_type').val();
		    var openid = $('#openid').val();
		    var applicationuserId = $('#applicationuserId').val();
			if(!$('#realname').val()){Msg('请输入联系人姓名',2);return;}
			if(!$('#phone').val()){Msg('请输入手机号',2);return;}
			else if(!checkTel($('#phone').val())){Msg('手机格式错误',2);return;}
			if(!$('#address').val()){Msg('请输入详细地址',2);return;}
			if($('#address').val().length<6){Msg('详细地址不能少于六个字',2);return;}
			var id = $('#addressId').val();
			var contactPhoneNum = $('#phone').val();
			var contact = $('#realname').val();
			//var addr = $('#selCity').val() + $('#selDistrict').val() + $('#address').val();
			var addr = $('#address').val();
		    var countyId= $('#selDistrict').val();
			var isDefault = $('#isDefault').val();
			var data = { id: id, contactPhoneNum: contactPhoneNum, contact: contact, addr: addr, isDefault: isDefault, applicationuserId: applicationuserId, countyId: countyId }

			ajaxLocal('/ajax/AddOrEditAddress?type=' + addType, data, function (json) {
				if(json.Valid){
					Address.BackPage('address_add');
					setTimeout(function () { pageTurn('/frontpage/AddAddress?openid=' + openid); }, 320);
				}else{
					Msg(json.Msg,2);
				}
			});
		});

	


	},

	TurnPage:function(obj){
		$('section.address_add').show();
		setTimeout(function(){
			transform($('section.address_add'),'-100%');
			transform($('section.address_list'),'-100%');
		},10);
		setTimeout(function(){
			$('section.address_list').hide();
		},420);
		if(obj){
			Address.printAddBox(obj);
		}
	},

	BackPage:function(type){
		$('section.address_list').show();
		setTimeout(function(){
			transform($('section.address_list'),'0');
			transform($('section.'+type),'0');
		},10);
		setTimeout(function(){
			$('section.'+type).hide();
		},420);
	},

	printAddBox:function(obj){
		if(obj){
			districtId = obj.attr('data-districtId');
			$('#add_id').val(obj.attr('data-id'));
			$('#realname').val(obj.attr('data-name'));
			$('#phone').val(obj.attr('data-mobile'));
			$('#selCity').val(obj.attr('data-cityid'));
			$('#selCity').change();
			$('#address').val(obj.attr('data-note'));
			Address.setDefault(obj.attr('data-default'));
		}else{
			districtId = 0;
			$('#add_id').val('');
			$('#realname').val('');
			$('#phone').val('');
			$('#selCity').val(store('cityId'));
			$('#selCity').change();
			$('#address').val('');
			Address.setDefault();
		}
	},

	setDefault:function(v){
		if(v=="True"){
			$('#isDefault').val(1);
			$('ul.add_add li.def span').addClass('sel');
		}else{
			$('#isDefault').val(0);
			$('ul.add_add li.def span').removeClass('sel');
		}
	},
		
	/**
	* @ 页面初始化
	*/		
	init_page:function(type){
		Address.defaultClick();
	}
}