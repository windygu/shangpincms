/**
 * @author Macrox
 */
;
(function (window, $) {
    var frameFix = (function () {
        var framing = $("<iframe>").attr({
                "id":"sp_framefix",
                "name":"sp_framefix",
                "frameborder":0,
                "marginwidth":0,
                "marginheight":0,
                "scrolling":"no"
            }).css({
                    "position":"absolute",
                    "display":"none",
                    "z-index":999
                }),
            init,
            install = function () {
                if (init) {
                    return true;
                }
                $("body").append(framing);
                return init = true;
            };

        return {
            get:function () {
                return framing;
            },
            hide:function () {
                framing.hide();
                return this;
            },
            bindTo:function (o, changeZIndex) {
                if (!init) {
                    install();
                }
                if (!o) {
                    return this;
                }
                var obj = o,
                    left = 0,
                    top = 0,
                    width = 0,
                    height = 0,
                    offset,
                    frameobj = framing,
                    zindex = 1000;

                if (obj && obj.size() == 1) {
                    offset = obj.offset();
                    left = offset.left;
                    top = offset.top;
                    width = o.width();
                    height = o.height();

                    frameobj.css({
                        "left":left + "px",
                        "top":top + "px",
                        "height":height + "px",
                        "width":width + "px",
                        "opacity":0
                    }).show();

                    if (changeZIndex) {
                        zindex = Math.floor(new Date().getTime() / 1000);
                        obj.css({
                            "z-index":zindex + 10
                        });
                    }
                }

                return this;
            }
        }
    })();
    var modeWindow = (function () {
        var defaultSetting = {
                id:null,
                style:{
                    wrapper:null,
                    title:null,
                    content:null,
                    bottom:null
                },
                html:{
                    title:null,
                    content:null,
                    bottom:null
                },
                position:{
                    init:"center",
                    relat:{
                        top:null,
                        left:0
                    }
                },
                animation:{
                    init:function () {
                        this.hide();
                    },
                    show:function () {
                        this.show();
                    },
                    close:function () {
                        this.hide();
                    }
                },
                drag:false,
                backover:false
            },
            setElemPosition = {
                "center":function (elem, relat) {
                    var win = $(window),
                        doc = $(document),
                        elemWidth = elem.outerWidth(),
                        elemHeight = elem.outerHeight(),
                        winWidth = win.width(),
                        winHeight = win.height(),
                        scrollTop = win.scrollTop(),
                        scrollLeft = win.scrollLeft(),
                        centerTop = winHeight / 2 - elemHeight / 2,
                        centerLeft = winWidth / 2 - elemWidth / 2,
                        finalTop = centerTop + scrollTop + (relat.top || 0),
                        finalLeft = centerLeft + scrollLeft + (relat.left || 0);

                    if (finalTop < 0) {
                        finalTop = 0;
                    }
                    if (finalLeft < 0) {
                        finalLeft = 0;
                    }

                    //msg(centerTop + ":" + relat.top + ":" + finalTop + ":" + elemHeight);
                    elem.css({
                        "position":"absolute",
                        "left":finalLeft,
                        "top":finalTop
                    });
                },
                "nearby":function (elem, near, relat) {
                    var offset = near.offset(),
                        nearTop = offset.top,
                        nearLeft = offset.left,
                        finalTop = nearTop + (relat.top || 0),
                        finalLeft = nearLeft + (relat.left || 0);

                    if (finalTop < 0) {
                        finalTop = 0;
                    }
                    if (finalLeft < 0) {
                        finalLeft = 0;
                    }

                    elem.css({
                        "position":"absolute",
                        "left":finalLeft,
                        "top":finalTop
                    });
                }
            },
            createContainer = function (id, label, style) {
                var ret = $("<" + label + ">")[(function (id) {
                        return !id ? "data" : "attr";
                    })(id)]("id", id),
                    attr;

                if (style) {
                    attr = typeof style === "string" ? "addClass" : "css";
                    ret[attr](style);
                }

                return ret;
            },
            bindDragEvent = (function () {
                var dragObj;
                bindDragEvent = function (elem, dragarea) {
                    dragarea.bind("mousedown", function (e) {
                        var offs = elem.offset(),
                            doc = $(document);
                        if (dragObj) {
                            dragObj = null;
                        }
                        elem.data({
                            "dragable":true,
                            "top":offs.top,
                            "left":offs.left,
                            "mousetop":e.pageY,
                            "mouseleft":e.pageX,
                            "relatX":e.pageX - offs.left,
                            "relatY":e.pageY - offs.top,
                            "mintop":0,
                            "minleft":0,
                            "maxtop":doc.innerHeight() - elem.outerHeight(),
                            "maxleft":doc.innerWidth() - elem.outerWidth()
                        });
                        dragObj = elem;
                        return false;
                    });
                };

                $(document)
                    .bind("mousemove", function (e) {
                        if (dragObj && dragObj.data("dragable")) {
                            var mouseX = e.pageX,
                                mouseY = e.pageY,
                                relatX = dragObj.data("relatX"),
                                relatY = dragObj.data("relatY"),
                                calcX = mouseX - relatX,
                                calcY = mouseY - relatY,
                                mintop = dragObj.data("mintop"),
                                minleft = dragObj.data("minleft"),
                                maxtop = dragObj.data("maxtop"),
                                maxleft = dragObj.data("maxleft");

                            //msg("relatX: " + relatX + " relatY: " + relatY);

                            if (calcX < minleft) {
                                calcX = minleft;
                            }
                            if (calcX > maxleft) {
                                calcX = maxleft;
                            }
                            if (calcY < mintop) {
                                calcY = mintop;
                            }
                            if (calcY > maxtop) {
                                calcY = maxtop;
                            }

                            dragObj.css({
                                "left":calcX + "px",
                                "top":calcY + "px"
                            });

                            return false;
                        }
                    })
                    .bind("mouseup", function (e) {
                        if (dragObj) {
                            dragObj = null;
                        }
                    });

                return bindDragEvent;
            })(),
            backoverObj,
            createBackover = function () {
                var zindex = Math.floor(new Date().getTime() / 1000);
                backover = $("<div>").css({
                    "position":"absolute",
                    "background-color":"#000",
                    "opacity":0.6,
                    "z-index":zindex
                });

                $("body").append(backover);

                return backover;
            },
            zIndexedElem = function (elem) {
                var zindex = Math.floor(new Date().getTime() / 1000) + 1;
                elem.css("z-index", zindex);
            },
            modeWindow = function (setting) {
                var _setting = $.extend(true, {}, defaultSetting, setting);

                this.init(_setting);
            };
        modeWindow.prototype = {
            constructor:modeWindow,

            applyWindow:function () {
                var wrapper = this.getWrapper(),
                    title = this.getTitle(),
                    content = this.getContent(),
                    bottom = this.getBottom();

                this.setHTML();
                //初始化一下wrapper
                this._animation.init.call(wrapper);

                $("body")
                    .append(
                    wrapper
                        .append(title)
                        .append(content)
                        .append(bottom)
                );

                this.drag();
                this._binder && this._binder.call(this);

                return this;
            },

            setHTML:function () {
                var title = this.getTitle(),
                    content = this.getContent(),
                    bottom = this.getBottom(),
                    titleHTML = this.getTitleHTML(),
                    contentHTML = this.getContentHTML(),
                    bottomHTML = this.getBottomHTML();

                title.html($.isFunction(titleHTML) ? titleHTML.call(this) : titleHTML);
                content.html($.isFunction(contentHTML) ? contentHTML.call(this) : contentHTML);
                bottom.html($.isFunction(bottomHTML) ? bottomHTML.call(this) : bottomHTML);

                return this;
            },

            init:function (setting) {
                var id = setting.id,
                    style = setting.style,
                    html = setting.html,
                    position = setting.position,
                    animation = setting.animation,
                    drag = setting.drag,
                    backover = setting.backover,
                    binder = setting.binder,

                //筛选样式
                    wrapperStyle = style.wrapper,
                    titleStyle = style.title,
                    contentStyle = style.content,
                    bottomStyle = style.bottom,

                //筛选内容
                    titleHTML = html.title,
                    contentHTML = html.content,
                    bottomHTML = html.bottom,

                //创建各种容器
                    wrapper = createContainer(id, "div", wrapperStyle),
                    title = createContainer(null, "div", titleStyle),
                    content = createContainer(null, "div", contentStyle),
                    bottom = createContainer(null, "div", bottomStyle),
                    that = this;

                //先做成公共属性
                this._position = position;
                this._animation = animation;
                this._dragable = drag;
                this._backover = backover;
                this._binder = binder;

                //protected方法
                this.getWrapper = function () {
                    return wrapper;
                };

                this.getTitle = function () {
                    return title;
                };

                this.getContent = function () {
                    return content;
                };

                this.getBottom = function () {
                    return bottom;
                };

                this.getTitleHTML = function () {
                    return titleHTML;
                };

                this.setTitleHTML = function (val) {
                    titleHTML = val;
                };

                this.getContentHTML = function () {
                    return contentHTML;
                };

                this.setContentHTML = function (val) {
                    contentHTML = val;
                };

                this.getBottomHTML = function () {
                    return bottomHTML;
                };

                this.setBottomHTML = function (val) {
                    bottomHTML = val;
                };

                //生成window的实例
                this.applyWindow();
            },

            position:function () {
                var position = this._position,
                    initPosition = position.init,
                    relatPosition = arguments[0] || position.relat,
                    wrapper = this.getWrapper();

                if (initPosition instanceof $) {
                    setElemPosition.nearby.call(this, wrapper, initPosition, relatPosition);
                }
                else {
                    if (!$.isFunction(initPosition)) {
                        initPosition = setElemPosition[initPosition] || null;
                    }

                    initPosition && initPosition.call(this, wrapper, relatPosition);
                }
                return this;
            },

            reposition:function (position) {
                this._position = $.extend(true, {}, this._position, position);
                return this;
            },
			
			resize:function(w,h,fn){
		        var resizeAnim = this.getWrapper(),
				    iL = parseFloat(resizeAnim.css("left")),
					iT = parseFloat(resizeAnim.css("top")),
					iW = resizeAnim.width(),
					iH = resizeAnim.height(),
				    dL = (iW - w) / 2 + iL,
					dT = (iH - h) / 2 + iT,
					that = this;
				resizeAnim.animate({
					"width":w + "px",
					"height":h + "px",
				    "left":dL + "px",
					"top":dT + "px"
			    },function(){
					fn && fn.call(that);
				});
			    return this;	
			},

            show:function (fn) {
                var showAnim = this._animation.show,
                    wrapper = this.position().getWrapper(),
                    that = this;

                this.backover();
                zIndexedElem(wrapper);

                setTimeout(function () {
                    frameFix.bindTo($("body"), false);
                    showAnim && showAnim.call(wrapper);
                    fn && fn.call(wrapper, that);
                }, 0);

                return this;
            },

            close:function (closebackover) {
                var closeAnim = this._animation.close,
                    wrapper = this.getWrapper();

                closeAnim && closeAnim.call(wrapper);
                frameFix.hide();

                if (backoverObj) {
                    //如果传入参数为false，即不关闭遮罩层，则判断是否当前是最后一个窗口
                    //如果是最后一个窗口，则将其bindto置空
                    if(typeof closebackover === "undefined"){
                        if (backoverObj.data("bindto") == wrapper){
                            backoverObj.fadeOut(200).data("bindto", null);
                        }
                    }
                    else if (closebackover === false) {
                        if (backoverObj.data("bindto") == wrapper) {
                            backoverObj.data("bindto", null);
                        }
                    }
                    else{
                        backoverObj.fadeOut(200).data("bindto", null);
                    }
                }

                return this;
            },

            setContent:function (val, reposition) {
                var that = this;
                this.getContent().html(val);
                if (reposition) {
                    setTimeout(function () {
                        that.position(reposition);
                    }, 0);
                }
            },

            drag:function (area) {
                if (this._dragable) {
                    var elem = this.getWrapper(), defaultArea = this.getTitle(), that = this, dragarea = area ? (function () {
                        return $.isFunction(area) ? area.call(that) : defaultArea;
                    })() : defaultArea;

                    bindDragEvent(elem, dragarea);
                }
                return this;
            },

            backover:function (isbackover) {
                var backover = this._backover = typeof isbackover == "undefined" ? this._backover : !!isbackover;

                if (backover) {
                    var win = $(window), doc = $(document), winWidth = win.width(), winHeight = win.height(), docWidth = doc.width(), docHeight = doc.height(), backoverWidth = winWidth > docWidth ? winWidth : docWidth, backoverHeight = winHeight > docHeight ? winHeight : docHeight;

                    if (!backoverObj) {
                        backoverObj = createBackover();
                    }

                    if (!backoverObj.data("bindto")) {
                        backoverObj.data("bindto", this.getWrapper());
                    }

                    backoverObj.css({
                        "top":0,
                        "left":0,
                        "width":backoverWidth + "px",
                        "height":backoverHeight + "px"
                    }).show();
                }

                return backover;
            }
        };

        return modeWindow;
    })();

    window.SPModeWindow = function (setting) {
        return new modeWindow(setting);
    };
    window.SPModeWindowCollection = (function () {
        var windows = {};

        return {
            add:function (id, setting) {
                if (!windows[id]) {
                    windows[id] = new modeWindow(setting);
                }
                return windows[id];
            },

            get:function (id) {
                return windows[id] || null;
            }
        };
    })();
})(window, jQuery);