// JavaScript Document
/* <![CDATA[ */
(function($) {
	function cur(ele,currentClass,tag) {
		ele= $(ele)? $(ele):ele;
		if(!tag) {
			ele.addClass(currentClass).siblings().removeClass(currentClass);
		} else {
			ele.addClass(currentClass).siblings(tag).removeClass(currentClass);
		}
	}

	$.fn.spTabs = function(options) {
		var org= {
			tabId: "",	//选择卡id
			tabTag: "",	//选择卡子标签 eg:li
			conId: "",	//内容id
			conTag: "",	//内容子标签 eg:div
			curClass: "cur",	//选择卡hover className
			loadTxt: '数据加载中...', //ajax等待字符串
			act: "click",	//鼠标事件 eg:mouseover
			dft: 0,	//默认初始显示位置 index
			effact: null,	//切换效果 eg:scrolly,scrollx,fade等，默认为show
			auto: false,	//自动轮播
			autotime: 3000,	//轮播间隔时间
			aniSpeed: 500	//动画执行时间
		};
		$.extend(org,options);
		var t=false;
		var k=0;
		var _this=$(this);
		var tagwrp=$(org.tabId);
		var conwrp=$(org.conId);
		var tag=tagwrp.find(org.tabTag);
		var con=conwrp.find(org.conTag);
		var len=tag.length;
		var taght=parseInt(tagwrp.css("height"));
		var conwh=conwrp.get(0).offsetWidth;
		var conht=conwrp.get(0).offsetHeight;
		var curtag=tag.eq(org.dft);
		//prepare
		cur(curtag,org.curClass);
		con.eq(org.dft).show().siblings(org.conTag).hide();
		if(org.effact=="scrollx") {
			var padding=parseInt(con.css("padding-left"))+parseInt(con.css("padding-right"));
			_this.css({
				"position" :"relative",
				"height" :taght+conht+"px",
				"overflow" :"hidden"
			});
			conwrp.css({
				"width" :len*conwh+"px",
				"height" :conht+"px",
				"position" :"absolute",
				"top" :taght+"px"
			});
			con.css({
				"float" :"left",
				"width" :conwh-padding+"px",
				"display" :"block"
			});
		}
		if(org.effact=="scrolly") {
			var padding=parseInt(con.css("padding-top"))+parseInt(con.css("padding-bottom"));
			_this.css({
				"position" :"relative",
				"height" :taght+conht+"px",
				"overflow" :"hidden"
			});
			tagwrp.css({
				"z-index" :100
			})
			conwrp.css({
				"width" :"100%",
				"height" :len*conht+"px",
				"position" :"absolute",
				"z-index" :99
			})
			con.css({
				"height" :conht-padding+"px",
				"float" :"none",
				"display" :"block"
			});
		}
		tag.each( function(i) {
			tag.eq(i).bind(org.act, function() {
				cur(this,org.curClass);
				k=i;
				
				function ajax(div, li) {
					var i = div; //当前div
					var rel = li.attr("rel"); //ajax请求url
					
					if (rel) { //如果ajax请求url不为空
						i.html(org.loadTxt);
						$.ajax({
							url: rel,
							cache: false,
							success: function (html) {
								i.html(html);
								i.attr("changed","true");
							},
							error: function () {
								i.html("数据加载错误，请重试！");
							}
						});
					}
				}
				
				if (tag.eq(i).attr("rel")){
					ajax(con.eq(i), tag.eq(i));
				}
				
				if(con.eq(i).attr("changed")=="true"){
					tag.eq(i).removeAttr("rel");//只加载一次
				}
				
				switch(org.effact) {
					case "slow" :
						con.eq(i).show("slow").siblings(org.conTag).hide("slow");
						break;
					case "fast" :
						con.eq(i).show("fast").siblings(org.conTag).hide("fast");
						break;
					case "scrollx" :
						conwrp.animate({
							left:-i*conwh+"px"
						},org.aniSpeed);
						break;
					case "scrolly" :
						conwrp.animate({
							top:-i*conht+taght+"px"
						},org.aniSpeed);
						break;
					case "fade" :
						con.eq(i).fadeIn(org.aniSpeed).siblings(org.conTag).hide("fast");
						break;
					default :
						con.eq(i).show().siblings(org.conTag).hide();
						break;
					//End of switch
				}
			}
			)
		})

		if(org.auto) {
			var drive= function() {
				if(org.act=="mouseover") {
					tag.eq(k).mouseover();
				} else if(org.act=="click") {
					tag.eq(k).click();
				}
				k++;
				if(k==len)
					k=0;
			}
			t = setInterval(drive,org.autotime);
			//鼠标滑过停止滚动
			$(org.conTag).hover(function(){
				clearInterval(t);
			},function(){
				t = setInterval(drive,org.autotime);
			});
		}
		//End of $.fn.spTabs
	}
})(jQuery);
/* ]]> */