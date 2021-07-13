var indexAssignment = function () {

    this.initialize = function () {
        registerEvents();

    };
    function registerEvents() {
 
    }
};
$(document).ready(function () {
    $(".select2").select2({
        dropdownAutoWidth: true,
        width: '100%',
        placeholder: "Choose Staff",
        allowClear: true
    });
});