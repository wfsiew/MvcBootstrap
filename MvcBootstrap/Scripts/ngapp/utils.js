var utils = (function () {

    function getDateStr(date) {
        var s = '';

        if (date == null)
            return s;

        s = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
        return s;
    }

    function getDate(a) {
        if (a != null) {
            var v = a.replace('/Date(', '').replace(')/', '');
            var i = parseInt(v);
            var date = new Date(i);
            return date;
        }

        return null;
    }

    return {
        getDateStr: getDateStr,
        getDate: getDate
    };
}());