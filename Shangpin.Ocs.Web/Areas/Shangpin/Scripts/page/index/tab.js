/**
 * User: MacroXing
 * Date: 12-9-14
 * Time: 上午8:54
 */
(function (window, $, SP) {
    var Tab = (function () {
        var jqConvertor = function (s) {
                return s instanceof $ ? s : (function () {
                    var ret = $(s);
                    return ret.length == 0 ? null : ret;
                })();
            },
            tabSetter = function (last, current) {
                this.last(last);
                this.current(current);
            },
            defaultSetting = {
                wrapper: null,
                contentWrapper: null,
                first: null,
                trigger: "click",
                effector: "direct",
				curClass: "cur",//new
                onchange: null,
				auto: false,//new
				autotime: 3000//new
            },
            changeEvents = {
                "direct": function(){
                    var last = this.last(),
                        current = this.current();
                    this.getContent(last).stop(true).hide();
                    this.getContent(current).show();
                },
                "fade": function () {
                    var last = this.last(),
                        current = this.current();
                    this.getContent(last).stop(true).hide();
                    this.getContent(current).fadeIn(200);
                }
            },
            Tab = function (setting) {
                var _setting = $.extend({},
                        defaultSetting, setting),
                    last = null,
                    current = null,
                    onchange = _setting.onchange;
                this.getSetting = function () {
                    return $.extend({}, _setting, true);
                };
                this.last = function (indexer) {
                    if (typeof indexer !== "undefined") {
                        last = indexer;
                    }
                    return last;
                };
                this.current = function (indexer) {
                    if (typeof indexer !== "undefined") {
                        current = indexer;
                    }
                    return current;
                };
                this.onchange = function (fn) {
                    if (Object.prototype.toString.call(fn) === "Function") {
                        onchange = fn;
                    }
                    return onchange;
                };
                this.init();
            };
        Tab.prototype = {
            constructor: Tab,
            init: function () {
                var s = this.getSetting(),
                    wrapper = jqConvertor(s.wrapper),
                    contentWrapper = jqConvertor(s.contentWrapper),
                    trigger = s.trigger,
                    onchange = s.onchange,
                    that = this,
                    tabs = wrapper.find("*[tabid]"),
                    first,
					tabLen = tabs.length,//new
					isAuto = s.auto,//new
					timer = null,//new
					k = 0;//new
				this.wrapper = function(){
					return wrapper;
				};
                this.contentWrapper = function () {
                    return contentWrapper;
                };
                if (tabLen > 0 && wrapper != null) {
                    first = s.first || tabs.eq(0).attr("tabid");
                    that.change(first);
                    wrapper.delegate("[tabid]", trigger, function () {
                        var $this = $(this),
                            $tabid = $this.attr("tabid"),
							$conWrp = contentWrapper.find("[tabcontent='" + $tabid + "']");
                        that.change($tabid);
						k = $this.index();//k=当前索引
						
						
						//获取AJAX动态数据
						if ($this.attr("rel") && $conWrp.attr("changed") != "true"){
							ajax($conWrp, $this);
						}
						//获取AJAX动态数据
						
                    });
					
					//AJAX动态添加内容
					var ajax = function(div, li) {
						var i = div; //当前div
						var rel = li.attr("rel"); //ajax请求url
						
						if (rel) { //如果ajax请求url不为空
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
					//AJAX动态添加内容
					
					//自动切换
					if(isAuto) {
						var drive = function() {
								var $tabid = tabs.eq(k).attr("tabid"),
									$tabTag = wrapper.find("[tabid='" + $tabid + "']")
									$conWrp = contentWrapper.find("[tabcontent='" + $tabid + "']");
									
								that.change($tabid);
								
								//获取AJAX动态数据
								if ($tabTag.attr("rel") && $conWrp.attr("changed") != "true"){
									ajax($conWrp, $tabTag);
								}
								//获取AJAX动态数据
								
								k++;
								if (k == tabLen){
									k = 0;
								}
						}
						timer = setInterval(drive,s.autotime);
						//鼠标滑过停止滚动
						$(contentWrapper).hover(function(){
							clearInterval(timer);
						},function(){
							timer = setInterval(drive,s.autotime);
						});
					}
					//自动切换
                }
            },
            change: function (indexer) {
                var s = this.getSetting(),
					contentWrapper = s.contentWrapper,
                    onchange = this.onchange(),
                    current = this.current(),
                    effector,
					curCls = s.curClass;//new
                if (current !== indexer) {
                    effector = s.effector;
					//new tab样式切换
					this.getIndexer(indexer).addClass(curCls).siblings().removeClass(curCls);
					
                    tabSetter.call(this, current, indexer);
                    onchange && onchange.call(this);
                    if (typeof effector == "string" && typeof changeEvents[effector] != "undefined") {
                        changeEvents[effector].call(this);
                    }
                    else if($.isFunction(effector)){
                        effector.call(this);
                    }
                }
            },
			getIndexer: function(indexer){
				return this.wrapper().find("[tabid='" + indexer + "']");
			},
            getContent: function (indexer) {
				return this.contentWrapper().find("[tabcontent='" + indexer + "']");
			}
        };
        return Tab;
    })();
    SP.plug.tab = {
        create: function (setting) {
            return new Tab(setting);
        }
    };
})(window, jQuery, SP, undefined);
