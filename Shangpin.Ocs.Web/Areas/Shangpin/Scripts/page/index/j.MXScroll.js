/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 12-11-28
 * Time: 上午9:33
 * To change this template use File | Settings | File Templates.
 */
(function (window, $) {
    /**
     * API
     *
     */
    var MXScroll = (function () {
        //内置缓动库
        $.extend($.easing, {
            def: 'easeOutQuad',
            easeInQuad: function (x, t, b, c, d) {
                return c * (t /= d) * t + b;
            },
            easeOutQuad: function (x, t, b, c, d) {
                return -c * (t /= d) * (t - 2) + b;
            },
            easeInOutQuad: function (x, t, b, c, d) {
                if ((t /= d / 2) < 1) {
                    return c / 2 * t * t + b;
                }
                return -c / 2 * ((--t) * (t - 2) - 1) + b;
            },
            easeInCubic: function (x, t, b, c, d) {
                return c * (t /= d) * t * t + b;
            },
            easeOutCubic: function (x, t, b, c, d) {
                return c * ((t = t / d - 1) * t * t + 1) + b;
            },
            easeInOutCubic: function (x, t, b, c, d) {
                if ((t /= d / 2) < 1) {
                    return c / 2 * t * t * t + b;
                }
                return c / 2 * ((t -= 2) * t * t + 2) + b;
            },
            easeInQuart: function (x, t, b, c, d) {
                return c * (t /= d) * t * t * t + b;
            },
            easeOutQuart: function (x, t, b, c, d) {
                return -c * ((t = t / d - 1) * t * t * t - 1) + b;
            },
            easeInOutQuart: function (x, t, b, c, d) {
                if ((t /= d / 2) < 1) {
                    return c / 2 * t * t * t * t + b;
                }
                return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
            },
            easeInQuint: function (x, t, b, c, d) {
                return c * (t /= d) * t * t * t * t + b;
            },
            easeOutQuint: function (x, t, b, c, d) {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
            },
            easeInOutQuint: function (x, t, b, c, d) {
                if ((t /= d / 2) < 1) {
                    return c / 2 * t * t * t * t * t + b;
                }
                return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
            },
            easeInSine: function (x, t, b, c, d) {
                return -c * Math.cos(t / d * (Math.PI / 2)) + c + b;
            },
            easeOutSine: function (x, t, b, c, d) {
                return c * Math.sin(t / d * (Math.PI / 2)) + b;
            },
            easeInOutSine: function (x, t, b, c, d) {
                return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b;
            },
            easeInExpo: function (x, t, b, c, d) {
                return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b;
            },
            easeOutExpo: function (x, t, b, c, d) {
                return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;
            },
            easeInOutExpo: function (x, t, b, c, d) {
                if (t == 0) {
                    return b;
                }
                if (t == d) {
                    return b + c;
                }
                if ((t /= d / 2) < 1) {
                    return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
                }
                return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;
            },
            easeInCirc: function (x, t, b, c, d) {
                return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b;
            },
            easeOutCirc: function (x, t, b, c, d) {
                return c * Math.sqrt(1 - (t = t / d - 1) * t) + b;
            },
            easeInOutCirc: function (x, t, b, c, d) {
                if ((t /= d / 2) < 1) {
                    return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
                }
                return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b;
            },
            easeInElastic: function (x, t, b, c, d) {
                var s = 1.70158;
                var p = 0;
                var a = c;
                if (t == 0) {
                    return b;
                }
                if ((t /= d) == 1) {
                    return b + c;
                }
                if (!p) {
                    p = d * .3;
                }
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4;
                }
                else {
                    var s = p / (2 * Math.PI) * Math.asin(c / a);
                }
                return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
            },
            easeOutElastic: function (x, t, b, c, d) {
                var s = 1.70158;
                var p = 0;
                var a = c;
                if (t == 0) {
                    return b;
                }
                if ((t /= d) == 1) {
                    return b + c;
                }
                if (!p) {
                    p = d * .3;
                }
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4;
                }
                else {
                    var s = p / (2 * Math.PI) * Math.asin(c / a);
                }
                return a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b;
            },
            easeInOutElastic: function (x, t, b, c, d) {
                var s = 1.70158;
                var p = 0;
                var a = c;
                if (t == 0) {
                    return b;
                }
                if ((t /= d / 2) == 2) {
                    return b + c;
                }
                if (!p) {
                    p = d * (.3 * 1.5);
                }
                if (a < Math.abs(c)) {
                    a = c;
                    var s = p / 4;
                }
                else {
                    var s = p / (2 * Math.PI) * Math.asin(c / a);
                }
                if (t < 1) {
                    return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
                }
                return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
            },
            easeInBack: function (x, t, b, c, d, s) {
                if (s == undefined) {
                    s = 1.70158;
                }
                return c * (t /= d) * t * ((s + 1) * t - s) + b;
            },
            easeOutBack: function (x, t, b, c, d, s) {
                if (s == undefined) {
                    s = 1.70158;
                }
                return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
            },
            easeInOutBack: function (x, t, b, c, d, s) {
                if (s == undefined) {
                    s = 1.70158;
                }
                if ((t /= d / 2) < 1) {
                    return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
                }
                return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
            },
            easeInBounce: function (x, t, b, c, d) {
                return c - jQuery.easing.easeOutBounce(x, d - t, 0, c, d) + b;
            },
            easeOutBounce: function (x, t, b, c, d) {
                if ((t /= d) < (1 / 2.75)) {
                    return c * (7.5625 * t * t) + b;
                }
                else {
                    if (t < (2 / 2.75)) {
                        return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
                    }
                    else {
                        if (t < (2.5 / 2.75)) {
                            return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
                        }
                        else {
                            return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
                        }
                    }
                }
            },
            easeInOutBounce: function (x, t, b, c, d) {
                if (t < d / 2) {
                    return jQuery.easing.easeInBounce(x, t * 2, 0, c, d) * .5 + b;
                }
                return jQuery.easing.easeOutBounce(x, t * 2 - d, 0, c, d) * .5 + c * .5 + b;
            }
        });

        var defaultSetting = {
                //需要进行滚动渲染的父层元素
                wrapper: null,

                //滚动方式，默认为水平滚动
                //"horizon":水平滚动
                //"vertical":垂直滚动
                type: "horizon",

                //可视范围，单位:px
                showWidth: 0,
                showHeight: 0,

                //内容元素队列
                content: null,

                //内容元素大小，单位:px
                contentElemWidth: 0,
                contentElemHeight: 0,

                //滚动速率
                rate: 200,

                //动画效果
                easing: "swing",

                //初始化回调函数
                onInit: null,

                //触发滚动前执行函数
                onScrollStart: null,

                //触发滚动时执行函数
                onScroll: null,

                //触发滚动后执行函数
                onScrollEnd: null
            },
            jqWrap = function (o) {
                return (o && o instanceof $) ? o : $(o);
            },
            appendLi = function (parent, appender, style) {
                var $li = $("<li>").css(style).append(appender);
                parent.append($li);
                return $li;
            },
            expando = {},
            randomer = function (bitcount) {
                var base = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
                    ret = "";
                while (bitcount--) {
                    ret += base.charAt(Math.floor(Math.random() * 62));
                }
                return ret;
            },
            MXScroll = function (setting) {
                var _setting = $.extend({}, defaultSetting, setting),
                    _index = 0,
                    that = this;

                //对setting的访问接口
                this.getSetting = function (name) {
                    return _setting[name] || undefined;
                };

                //对index的访问和设置接口
                this.index = function (i) {
                    if (typeof i === "number" && i >= 0) {
                        return _index = Math.floor(i);
                    }
                    else {
                        return _index;
                    }
                };

                this.init();
            };

        MXScroll.prototype = {
            constructor: MXScroll,
            init: function () {
                var wrapper = this.getSetting("wrapper"),
                    type = this.getSetting("type"),
                    content = this.getSetting("content") || [],
                    showWidth = this.getSetting("showWidth"),
                    showHeight = this.getSetting("showHeight"),
                    contentElemWidth = this.getSetting("contentElemWidth"),
                    contentElemHeight = this.getSetting("contentElemHeight"),
                    oninit = this.getSetting("onInit"),
                    totalWidth = 0,
                    totalHeight = 0,
                    $originalChild = wrapper.children(),
                    $content = $("<ul>"),
                    $wrapperStyle = {
                        width: showWidth + "px",
                        height: showHeight + "px",
                        overflow: "hidden",
						position: "relative" //修复ie67下当子元素相对定位的是父元素无法截取问题
                    },
                    $contentElemStyle = {
                        width: contentElemWidth + "px",
                        height: contentElemHeight + "px"
                    },
                    _max = 0,
                    _min = 0,
                    _uuid = randomer(10),
                    _expando = expando[_uuid] = [];

                if (type === "horizon") {
                    $contentElemStyle["float"] = "left";
                    wrapper.addClass("clr");
                }
				
				content = $originalChild.toArray().concat(content);

				for (var i = 0, len = content.length; i < len; i++) {
					_expando.push(appendLi($content, content[i], $contentElemStyle));
					totalWidth += contentElemWidth;
					totalHeight += contentElemHeight;
					_max++;
				}

				if (type === "horizon") {
					totalHeight = contentElemHeight;
				}
				else {
					totalWidth = contentElemWidth;
				}
				
				wrapper.css($wrapperStyle).append($content.css({width: totalWidth + "px", height: totalHeight + "px", position: "relative"}));

                this.max = function () {
                    return _max - 1;
                };

                this.min = function () {
                    return _min;
                };

                this.uuid = function () {
                    return _uuid;
                };

                this.position = 0;

                oninit && oninit.call(this);
            },
            currentElem: function () {
                return expando[this.uuid()][this.index()];
            },
            getElem: function (index) {
                return expando[this.uuid()][index || this.index()];
            },
            scrollTo: function (index, relat, events) {
                events = events || {};
                var wrapper = this.getSetting("wrapper"),
                    type = this.getSetting("type"),
                    position = type === "horizon" ? "left" : "top",
                    scroll = "scroll" + position.replace(/^(\w)/gi, function (w) {
                        return w.toUpperCase();
                    }),
                    final = 0,
                    elem,
                    onScrollStart = events.onScrollStart || this.getSetting("onScrollStart"),
                    onScroll = events.onScroll || this.getSetting("onScroll"),
                    onScrollEnd = events.onScrollEnd || this.getSetting("onScrollEnd"),
                    that = this;

                if ($.isFunction(onScrollStart)) {
                    if (!onScrollStart.call(this, index)) {
                        return false;
                    }
                }

                if (typeof index === "number") {
                    //永不越界
                    index = index < 0 ? 0 : (function () {
                        var max = that.max();
                        return index > max ? max : index;
                    })();
                    this.index(index);
                    elem = this.getElem(index);
                    final = elem.position()[position];
                }
                else {
                    final = parseInt(index);
                }

                //this.position = final += this.position;
                onScroll && onScroll.apply(this);

                final += typeof relat === "number" ? relat : 0;
                //wrapper.animate(({}[scroll] = final), 200);
                /*
                 wrapper.animate((function () {
                 var ret = {};
                 ret[scroll] = final;
                 return ret;
                 })(), this.getSetting("rate") || 200, this.getSetting("easing"), function(){
                 onScrollEnd && onScrollEnd.apply(that);
                 });
                 */
                wrapper.find("ul").stop(true, true).animate((function () {
                    var ret = {};
                    ret[position] = -final;
                    return ret;
                })(), this.getSetting("rate") || 200, this.getSetting("easing"), function () {
                    onScrollEnd && onScrollEnd.apply(that);
                })
            },
            next: function (circle, relat, events) {
                var index = this.index();
                circle = typeof circle === "undefined" ? true : !!circle;
                if (++index > this.max()) {
                    if (circle) {
                        index = this.min();
                    }
                    else {
                        return false;
                    }
                }
                this.scrollTo(index, relat, events);
            },
            prev: function (circle, relat, events) {
                var index = this.index();
                circle = typeof circle === "undefined" ? true : !!circle;
                if (--index < this.min()) {
                    if (circle) {
                        index = this.max();
                    }
                    else {
                        return false;
                    }
                }
                this.scrollTo(index, relat, events);
            }
        };

        return function (setting) {
            return new MXScroll(setting);
        };
    })();

    $.extend({"MXScroll": MXScroll});
    $.fn.extend({"MXScroll": function (setting) {
        setting.wrapper = this;
        return MXScroll(setting);
    }});
})(window, jQuery, undefined);