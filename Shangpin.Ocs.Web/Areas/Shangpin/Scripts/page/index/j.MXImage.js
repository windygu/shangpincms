/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-3-13
 * Time: 下午5:55
 * To change this template use File | Settings | File Templates.
 */
(function (window, $, undefined) {

    var MXImage = (function () {

        var defaultsetting = {
                imgqueue: null,
                preload: null,
                onload: null,
                always: null,
                preloadargs: null,
                onloadargs: null,
                alwaysargs: null
            },
            MXImage = function (setting) {
                var _setting = $.extend(true, {}, defaultsetting, setting);

                this._ = {
                    "setting": _setting,
                    "index": -1,
                    "last": -1,
                    "queue": undefined
                };
            };

        MXImage.fn = MXImage.prototype = {
            //reset constructor
            constructor: MXImage,

            //初始化或读取图片结构队列
            queue: function () {
                var _ = this._,
                    queue = _.queue,
                    settingqueue;

                //如果已经初始化了，就直接返回初始化的值
                if (typeof queue !== "undefined") {
                    return queue;
                }

                settingqueue = _.setting.imgqueue;
                //如果queue没有赋值，就返回null
                if (!settingqueue) {
                    queue = _.queue = null;
                    return queue;
                }

                //将输入的queue序列结构化
                queue = [];
                for (var i = 0, len = settingqueue.length; i < len; i++) {
                    queue.push({
                        "src": settingqueue[i],
                        "status": 0, //0:init, 1:requested, 2:loaded
                        "img": null,
                        "index": i
                    });
                }
                if (queue.length > 0) {
                    _.index = 0;
                }
                _.queue = queue;
                return queue;
            },

            //设置或读取当前的数字索引
            index: function (i) {
                if (this.isvalid(i)) {
                    this._.index = i;
                }

                return this._.index;
            },

            //返回图片队列的长度
            len: function () {
                var queue = this.queue();
                if (!queue) {
                    return -1;
                }

                return queue.length;
            },

            //判断输入索引是否有效
            isvalid: function (i) {
                var len = this.len(),
                    max = len - 1;

                if (max < 0 || typeof i !== "number") {
                    return false;
                }

                return i >= 0 && i <= max;
            },

            //获取相应索引的元素
            get: function (i) {
                var queue;

                if (!this.isvalid(i)) {
                    return null;
                }

                queue = this.queue();
                if (!queue) {
                    return null;
                }

                return queue[i];
            },

            //设置或获取当前元素
            current: function (i) {
                var index = this.index(i);
                return this.get(index);
            },

            //设置或获取上一索引元素
            last: function (i) {
                if (this.isvalid(i)) {
                    this._.last = i;
                }

                return this.get(this._.last);
            },

            next: function (isloop) {
                var index = this.index(),
                    max = this.len() - 1;

                isloop = typeof isloop === "undefined" ? true : isloop;

                if (max < 0) {
                    return -1;
                }
                if (++index > max) {
                    if (isloop) {
                        return 0;
                    }
                }

                return index;
            },

            prev: function (isloop) {
                var index = this.index(),
                    max = this.len() - 1,
                    min = 0;

                isloop = typeof isloop === "undefined" ? true : isloop;


                if (--index < min) {
                    if (isloop) {
                        return max;
                    }
                }

                return index;
            },

            //load图片，调用静态方法
            load: function (i, overrides) {
                var overridesMethod = $.extend(true, {}, this._.setting, overrides),
                    preload = overridesMethod.preload,
                    onload = overridesMethod.onload,
                    always = overridesMethod.always,
                    preloadargs = overridesMethod.preloadargs,
                    onloadargs = overridesMethod.onloadargs,
                    alwaysargs = overridesMethod.alwaysargs,
                    index = this.index(),
                    current,
                    that = this;

                if (i !== index || this._.last < 0) {
                    this.last(index);
                    current = this.current(i);

                    preload && preload.apply(this, Array.prototype.concat.call([current], preloadargs));

                    if (current) {
                        if (current.status === 0) {
                            current.status = 1;
                            MXImage.load(
                                current.src,
                                function (imgobj) {
                                    current.status = 2;
                                    current.img = $(imgobj);
                                    onload && onload.apply(that, Array.prototype.concat.call([current], onloadargs));
                                    always && always.apply(that, Array.prototype.concat.call([current], alwaysargs));
                                }
                            );
                        }
                        else if (current.status === 2) {
                            always && always.apply(this, Array.prototype.concat.call([current], alwaysargs));
                        }
                    }
                }

                return this;
            },

            each: function(fn) {
                var queue= this.queue();
                if ($.isFunction(fn)) {
                    for (var i = 0, len = queue.length; i < len; i++){
                        (function(_i){
                            fn.call(queue[_i], i);
                        })(i);
                    }
                }

                return this;
            },

            append: function (o) {
                var len = this.len(),
                    structor = {
                        "src": null,
                        "status": 0,
                        "img": null,
                        "index": len
                    };
                if (o instanceof $) {
                    //如果参数为jquery对象，就解析成队列元素对象
                    structor.src = o.attr("src");
                    structor.status = 2;
                    structor.img = o;
                }
                else {
                    structor = $.extend({}, structor, o);
                }
                this._.queue && this._.queue.push(structor);
            },

            prepend: function (o) {
                var structor = {
                        "src": null,
                        "status": 0,
                        "img": null,
                        "index": 0
                    };
                if (o instanceof $) {
                    //如果参数为jquery对象，就解析成队列元素对象
                    structor.src = o.attr("src");
                    structor.status = 2;
                    structor.img = o;
                }
                else {
                    structor = $.extend({}, structor, o);
                }
                this._.queue && this._.queue.unshift(structor);
                //重置index
                this.each(function(i){
                    this.index = i;
                });
            },

            replace: function (o, i) {
                if (!this.get(i)) {
                    return this;
                }
                var structor = {
                    "src": null,
                    "status": 0,
                    "img": null,
                    "index": i
                };
                if (o instanceof $) {
                    //如果参数为jquery对象，就解析成队列元素对象
                    structor.src = o.attr("src");
                    structor.status = 2;
                    structor.img = o;
                }
                else {
                    structor = $.extend({}, structor, o);
                }

                this._.queue[i] = structor;
                return this;
            }
        };

        //核心的Load方法，写成静态的，便于直接使用
        MXImage.load = function (src, fn, args, context) {
            var imgobj = new Image();

            if (!context) {
                context = null;
            }

            if (!args || !$.isArray(args)) {
                args = [];
            }

            args.unshift(imgobj);

            imgobj.onload = function () {
                imgobj.onload = null;
                fn && fn.apply(context, args);
            };

            imgobj.src = src;

            if (imgobj.complete) {
                imgobj.onload = null;
                fn && fn.apply(context, args);
            }

            return imgobj;
        };

        MXImage.Switchover = (function () {
            var defaultsetting = {
                    imgqueue: null,
                    wrapper: "#wrapper",
                    preload: function (structor, switcher) {
                        if (!switcher.last) {
                            switcher.last = this.last();
                        }
                    },
                    onload: function (structor, switcher) {
                        switcher.wrapper.append(structor.img.hide());
                    },
                    always: function (structor, switcher) {
                        var currentIndex = this.index(),
                            last = switcher.last;
                        if (structor.index === currentIndex) {
                            if (last && last.status === 2) {
                                last.img.stop(true, true).hide();
                                switcher.last = null;
                            }
                            structor.img.css("opacity", 1).fadeIn(1000);
                            switcher.autostart();
                        }
                    },
                    auto: 5000
                },
                Switchover = function (setting) {
                    var _setting = $.extend(true, {}, defaultsetting, setting, {preloadargs: [this], onloadargs: [this], alwaysargs: [this]});

                    this._ = {
                        setting: _setting
                    };

                    this.instance = new MXImage(_setting);
                    this.wrapper = $(_setting.wrapper);
                    this.autostatus = _setting.auto ? 1 : 0;
                    this.autotimer = null;
                    this.last = null;
                };

            Switchover.fn = Switchover.prototype = {

                constructor: Switchover,

                current: function (i, overrides) {
                    this.autoclear();
                    if (typeof i === "undefined") {
                        i = this.instance.index();
                    }

                    this.instance.load(i, overrides);
                },

                replace: function (o, i) {
                    this.instance.replace(o, i);
                },

                next: function (isloop) {
                    this.autoclear();
                    var index = this.instance.next(isloop);
                    this.current(index);
                },

                prev: function (isloop) {
                    this.autoclear();
                    var index = this.instance.prev(isloop);
                    this.current(index);
                },

                autostart: function () {
                    var auto = this._.setting.auto || 0,
                        that = this;
                    if (auto && this.autostatus) {
                        this.autotimer = window.setTimeout(function () {
                            that.next();
                        }, auto);
                    }

                    return this;
                },

                autostop: function () {
                    this.autostatus = 0;
                    this.autoclear();

                    return this;
                },

                autoclear: function () {
                    window.clearTimeout(this.autotimer);
                    this.autotimer = null;

                    return this;
                },

                autorestart: function () {
                    if (this._.setting.auto) {
                        this.autostatus = 1;
                        this.autostart();
                    }

                    return this;
                }

            };

            return function (setting) {
                return new Switchover(setting);
            };
        })();

        return MXImage;
    })();

    $.extend({
        "MXImage": MXImage
    })
})(window, jQuery);