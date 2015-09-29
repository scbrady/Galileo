$(function () {
    $('#team-projects').find('ul').each(function (i, ul) {
        var student_ids = '';
        var index = $(ul).data('project-index');

        $(ul).find('li').each(function (ii, li) {
            var user_id = $(li).attr('id');
            student_ids = student_ids + (!student_ids ? '' : ',') + user_id;
        });
        $('#hidden_' + index).val(student_ids);
    });

    $('.sortable').sortable({
        connectWith: '.connected'
    });

    $('.sortable').sortable().bind('sortupdate', function (e, ui) {
        var start_index = ui.startparent.data('project-index');
        var end_index = ui.endparent.data('project-index');

        var student_ids = '';

        if (typeof start_index != 'undefined') {
            ui.startparent.find("li").each(function (i, li) {
                var user_id = $(li).attr('id');
                student_ids = student_ids + (!student_ids ? '' : ',') + user_id;
            });
            $('#hidden_' + start_index).val(student_ids);
        }

        student_ids = '';
        ui.endparent.find("li").each(function (i, li) {
            var user_id = $(li).attr('id');
            student_ids = student_ids + (!student_ids ? '' : ',') + user_id;
        });
        $('#hidden_' + end_index).val(student_ids);
    });
});