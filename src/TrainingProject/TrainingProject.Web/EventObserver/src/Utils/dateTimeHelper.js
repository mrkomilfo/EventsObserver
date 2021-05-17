export default class DateTimeHelper {
    static getCurrentDateTime() {
        let today = new Date();
        let dd = today.getDate();
        let MM = today.getMonth()+1; //January is 0!
        let yyyy = today.getFullYear();
        let hh = today.getHours();
        let mm = today.getMinutes();
        
        if (dd < 10) {
            dd = '0' + dd;
        }

        if (MM < 10){
            MM = '0' + MM;
        }

        if (hh < 10){
            hh = '0' + hh;
        }

        if (mm < 10){
            mm = '0' + mm;
        }

        return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
    }
}