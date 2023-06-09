app.factory("commonService", [function () {
    var commonService = {};

    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var _formatDate = function (dateString) {
        var fullDate = new Date(dateString);
        var year = fullDate.getFullYear();
        var month = months[fullDate.getMonth()];
        var date = fullDate.getDate();
        return (date + " " + month + " " + year);
    }

    var _formatOnlyDate = function (dateString) {
        var fullDate = new Date(dateString);
        var year = fullDate.getFullYear();
        var month = fullDate.getMonth()+1;
        var date = fullDate.getDate();
        month = ('0' + month).slice(-2);
        date = ('0' + date).slice(-2);

        return (year + "-" + month + "-" + date );
    }

    var _formatDateTime = function (dateString) {
        var fullDate = new Date(dateString);
        var year = fullDate.getFullYear();
        var month = months[fullDate.getMonth()];
        var date = fullDate.getDate();
        var hour = fullDate.getHours();
        var min = fullDate.getMinutes();
        if (min < 10)
            min = "0" + min;
        return (date + " " + month + " " + year + ", " + hour + ":" + min);
    }

    var _roundToTwoDecimal = function (num) {
        return +(Math.round(num + "e+2") + "e-2");
    }

    var _scrollToPosition = function (Position, duration) {
        $('html,body').animate({
            scrollTop: $(Position).offset().top - 50
        }, duration);
    }

    var _scrollTop = function (Position, duration) {
        $("html, body").animate({
            scrollTop: 0
        }, "slow");
    }

    var _detectCardType = function (number) {
        var re = {
            electron: /^(4026|417500|4405|4508|4844|4913|4917)\d+$/,
            maestro: /^(5018|5020|5038|5612|5893|6304|6759|6761|6762|6763|0604|6390)\d+$/,
            dankort: /^(5019)\d+$/,
            interpayment: /^(636)\d+$/,
            unionpay: /^(62|88)\d+$/,
            visa: /^4[0-9]{12}(?:[0-9]{3})?$/,
            mastercard: /^5[1-5][0-9]{14}$/,
            amex: /^3[47][0-9]{13}$/,
            diners: /^3(?:0[0-5]|[68][0-9])[0-9]{11}$/,
            discover: /^6(?:011|5[0-9]{2})[0-9]{12}$/,
            jcb: /^(?:2131|1800|35\d{3})\d{11}$/
        };
        if (re.electron.test(number)) {
            return 'ELECTRON';
        } else if (re.maestro.test(number)) {
            return 'MAESTRO';
        } else if (re.dankort.test(number)) {
            return 'DANKORT';
        } else if (re.interpayment.test(number)) {
            return 'INTERPAYMENT';
        } else if (re.unionpay.test(number)) {
            return 'UNIONPAY';
        } else if (re.visa.test(number)) {
            return 'VISA';
        } else if (re.mastercard.test(number)) {
            return 'MASTERCARD';
        } else if (re.amex.test(number)) {
            return 'AMEX';
        } else if (re.diners.test(number)) {
            return 'DINERS';
        } else if (re.discover.test(number)) {
            return 'DISCOVER';
        } else if (re.jcb.test(number)) {
            return 'JCB';
        } else {
            return undefined;
        }
    }

    commonService.formatDate = _formatDate;
    commonService.formatOnlyDate = _formatOnlyDate;
    commonService.formatDateTime = _formatDateTime;
    commonService.roundToTwoDecimal = _roundToTwoDecimal;
    commonService.scrollToPosition = _scrollToPosition;
    commonService.scrollTop = _scrollTop;
    commonService.detectCardType = _detectCardType;
    return commonService;
}])
