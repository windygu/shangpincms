/**
 * @author Macrox
 */
(function (window, SP) {
	window.$Global = (function(){
		var storage = {};
		
		return {
			set: function(key, value, returnValue){
				storage[key] = value;
				return returnValue ? value : this;
			},
			get: function(key){
				return storage[key] || null;
			}
		};
	})();
	
	SP.page = {};
	
    //基础配置
    SP.config = {
        menu: null,
        shopcart: null,
        search: null,
        autoComplete: null,
        lazyload: null,
        weibo: {
            //appKey: "1646365302",
            //appSecret: "4c60482bbe786e3ff5bba604742030b7",
            //accessToken: "2.00HZEErBEpy6nB1109b703c4HsWp3C",
            appKey: "1004138071",
            appSecret: "b173d6963e046e2a1cccd664e635701a",
            accessToken: "2.00LqyWBC2LQxFB65a88dcf35RivAgD"
        }
    };
    $(function () {
        if (SP.plug) {
            //配置菜单项
            if (SP.plug.menu) {
                SP.config.menu = SP.plug.menu({
                    event: "hover",
                    title: {
                        obj: ["#sp_newHeader_type"],
                        on: function () {
                            $(this).addClass("on");
                        },
                        off: function () {
                            $(this).removeClass("on");
                        }
                    },
                    content: {
                        obj: ["#sp_newHeader_typeBox"],
                        fix: "#sp_newHeader"
                    }
                });
            }

            //配置搜索框
            if (SP.plug.search) {
                //新版头部搜索
                SP.config.newSearch = SP.plug.search({
                    elem: "#sp-newHeader-searchTxt",
                    text: {
                        //init: "输入搜索关键字"
                        //placeholder: "输入搜索关键字"
                    },
                    triggerSearch: {
                        elem: "#sp-newHeader-searchSb",
                        keyCode: [13]
                    },
                    render: {
                        focus: function () {
                            this.input.addClass("on");
                            this.clearVal(false, true);
                            $("#sp-newHeader-search").addClass("focus");
                        },
                        blur: function () {
                            this.input.removeClass("on");
                            this.resetVal();
                            $("#sp-newHeader-search").removeClass("focus");
                        },
                        onsearch: function () {
                            //window.location.href = "http://www.shangpin.com/search/search.html?SiteNo=&q=" + encodeURIComponent(this.getKeyword());
							window.location.href = "/html/search.new.html?key=" + encodeURIComponent(this.getKeyword()) + "&" + this.getCateid() + "&gender=0";
                            return;
                        }
                    }
                });
            }

            //配置搜索框自动完成
            if (SP.plug.autocomplete) {
                SP.config.autoComplete = SP.plug.autocomplete($("#sp-newHeader-searchTxt"), {left: -1, width: 6}, function () {
                    //与search组件轻耦合
                    SP.config.newSearch.setKeyword(null, this.cateid).search();
                }, function () {
					SP.config.newSearch.setKeyword(null, this.cateid);
				});
            }

            if (SP.plug.lazyload) {
                SP.config.lazyload = SP.plug.lazyload("img", null, null);
            }
        }

        //ipad点击事件
        $(document).bind("click", function (e) {
            var clickElem = e.target;
            if ($(clickElem).closest("#sp_newHeader_typeBox").length == 0) {
                if ($(clickElem).closest("#sp_newHeader_type").length > 0) {
                    $("#sp_newHeader_typeBox").toggle();
                }
                else{
                    $("#sp_newHeader_typeBox").hide();
                }
            }
            if (clickElem.id != "spDetail_b_buy") {
                if ($(clickElem).closest("#spPlug_shopcart").length == 0) {
                    $("#spPlug_shopcart").hide();
                    $("#spNewHeader_shopcart").removeClass("on");
                }
            }
        });
		
		//底部增加微信二维码hovor效果		
		$("#t_msgIcon").hover(function(){
		    $(this).toggleClass("color_fa");
		});
		
    });
})(window, SP, undefined);

/*新浪微博分享代码分离*/
var sinaShare = function (title, pic) {
    title = title.replace(/([\'\"])/gi, "");
    var _w = 72, _h = 16;
    var param = {
        url: location.href,
        type: '3',
        count: '1', /**是否显示分享数，1显示(可选)*/
        appkey: '3424885664', /**您申请的应用appkey,显示分享来源(可选)*/
        title: title, /**分享的文字内容(可选，默认为所在页面的title)*/
        pic: pic, /**分享图片的路径(可选)*/
        ralateUid: '1854902371', /**关联用户的UID，分享微博会@该用户(可选)*/
        language: 'zh_cn', /**设置语言，zh_cn|zh_tw(可选)*/
        rnd: new Date().valueOf()
    };
    var temp = [];
    for (var p in param) {
        temp.push(p + '=' + encodeURIComponent(param[p] || ''));
    }
    //return('<iframe allowTransparency="true" frameborder="0" scrolling="no" src="http://hits.sinajs.cn/A1/weiboshare.html?' + temp.join('&') + '" width="' + _w + '" height="' + _h + '"></iframe>')
    var shareSrc = "http://service.weibo.com/share/share.php?" + temp.join("&");
    return "<a href='#' id='sp-weibo-share' title='分享到新浪微博' target='_blank'><img src='/images/public/wb.gif' alt='分享到新浪微博' /></a>" +
           "<script type='text/javascript' charset='utf-8'>" +
           "document.getElementById('sp-weibo-share').onclick = function(e){e = e || window.event; 'preventDefault' in e ? e.preventDefault() : (function(){e.returnValue=false;})();var args=['toolbar=0,status=0,resizable=1,width=620,height=450,left=',(screen.width - 620)/2,',top=',(screen.height-450)/2].join('');window.open('" + shareSrc + "','spwb',args);};" +
           "</script>";
};

//搜索添加placeholder
$(function(){
	$('#sp-newHeader-searchTxt').placeHolder({
		defStyle:{color:"#999",left:"10px",fontSize:"12px",top:"0px",zIndex:"999"},
		focStyle:{color:"#ccc",fontSize:"12px",zIndex:"999"}
	});
});		