$(function(){
	
	
	var tabNav = $(".tabNav a"); //切换标签
	var tabsCont = $(".tabsCont"); //切换内容
	var nextStep= $(".nextStep");  //下一步
	var previousStep = $(".previousStep"); //上一步
	
	tabNav.bind("click", function(){  //tab切换
		var index = $(this).index();
		$(this).addClass("curr").siblings().removeClass("curr");
		tabsCont.eq(index).show().siblings().hide();
		return false;
	})

	nextStep.bind("click", function(){  //下一步切换
		var index = parseInt($(this).parents(".tabsCont").index())+1;
		tabsCont.eq(index).show().siblings().hide();
		tabNav.eq(index).addClass("curr").siblings().removeClass("curr");
		return false;
	})

	previousStep.bind("click", function(){  //上一步切换

		var index = parseInt($(this).parents(".tabsCont").index())-1;
		tabsCont.eq(index).show().siblings().hide();
		tabNav.eq(index).addClass("curr").siblings().removeClass("curr");
		return false;
	})

	
});