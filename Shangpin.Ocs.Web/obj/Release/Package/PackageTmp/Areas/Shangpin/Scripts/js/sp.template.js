/**
 * Created with JetBrains WebStorm.
 * User: MacroXing
 * Date: 13-7-19
 * Time: 下午1:02
 * To change this template use File | Settings | File Templates.
 */
;
(function (window, $, undefined) {
    var MXTempl = (function () {
        var MXTempl = function () {
            this.operator = null;
            this.renderred = null;
            this.data = null;
            this.html = null;
            this._render = null;
            this._renderExecute = 0;
            this._renderReady = null;
        };

        MXTempl.prototype = {
            constructor: MXTempl,
            requestData: function (url, payload) {
                var _this = this;
                $.ajax(
                    {
                        url: url,
                        type: "GET",
                        data: payload
                    }
                ).done(
                    function (data) {
                        _this.setData(data);
                    }
                );
                return this;
            },
            setData: function (data) {
                this.data = data;
                this._renderExecute = 1;
                if (this._render !== null) {
                    this._render.call(this);
                }
            },
            setHTML: function (html) {
                this.html = html;
            },
            renderReady: function (fn) {
                if (this._renderExecute == 2) {
                    fn && fn();
                }
                else {
                    this._renderReady = fn;
                }
                return this;
            },
            render: function (fn, args, context) {
                this._render = function () {
                    this._render = null;
                    this._renderExecute = 2;
                    if ($.isFunction(fn)) {
                        context = context || this;
                        args = args || [];
                        this.renderred = fn.apply(context, args);
                        this._renderReady && this._renderReady();
                    }
                };

                if (this._renderExecute > 0) {
                    this._render && this._render.call(this);
                }

                return this;
            },
            extend: function(key, value){
                MXTempl.prototype[key] = value;
                return this;
            }
        };

        MXTempl.cache = {};

        return function (id) {
            if (MXTempl.cache[id]) {
                return MXTempl.cache[id];
            }
            return MXTempl.cache[id] = new MXTempl();
        };
    })();

    window.MXTempl = MXTempl;
})(window, jQuery);