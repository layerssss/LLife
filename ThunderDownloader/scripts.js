


window.getList = function () {
    var result = {};
    try {
        $ = jQuery;
        if ($('[title][taskid]').length) {
            var list = [];
            $('[title][taskid]').each(function (i, e) {
                var title = $(e).attr('title');
                parant = $(e).closest('[taskid]');

                list.push(title);
            });
            result.list = list;
        }
    }
    catch (e) {
        result.message = e.Message;
    }
    return JSON.stringify(result);
};


