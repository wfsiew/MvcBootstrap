var utils = (function () {

    function getDateStr(date) {
        var s = '';

        if (date == null)
            return s;

        s = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
        return s;
    }

    return {
        getDateStr: getDateStr
    };
}());