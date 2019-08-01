var submitBttn = document.getElementById("click");
var calendar = document.getElementById('calendar'); 
var arr = [];
submitBttn.addEventListener("click", info);
function info(e){
    e.preventDefault();
    var dd = document.getElementById("StartDate");    
    var record = document.getElementById("CourseName");
    var s = document.getElementById("CourseId");
    var sd = document.getElementById("Total Hours");
    var dh =document.getElementById("DHours");
    var pr = document.getElementById("Prequisites");
    var v1 = sd.value;
    var v2 = dh.value;
    var totalDays = Math.ceil(v1/v2);
  
    var obj = {
      Title : record.value,
      Start: dd.value,
      TotalHours: sd.value,
      PerdayHours: dh.value,
      TotalDays: totalDays,
    }
    arr.push(obj);
    addCourse(obj.Start, obj.Title, obj.TotalDays)
    return obj;
   
} 

let addCourse = (startDate, courseName, totalDays) => {
  let dates = document.querySelectorAll('.fc-day')
  let dateArray = Array.from(dates)
  let date = startDate.split('-')
  let myDate = new Date(date[0], Number(date[1]), Number(date[2]))
  let day = 0
  console.log(totalDays)
  for (let i = 0; i < totalDays; i++) {
    let newDate = new Date(myDate.getTime() + day)
    day = day + 60 * 60 * 24 * 1000

    let dateObj = {
      date: newDate.getDate() <= 9 ? '0' + newDate.getDate() : newDate.getDate(),
      month: newDate.getMonth() <= 9 ? '0' + newDate.getMonth() : newDate.getMonth(),
      year: newDate.getFullYear()
    }
    //console.log((dateObj.year + '-' + dateObj.month + '-' + dateObj.date))
    dateArray.forEach(e => {
      if (e.dataset.date === (dateObj.year + '-' + dateObj.month + '-' + dateObj.date)) {
       e.textContent = courseName
      }
    })
  }
}

document.addEventListener('DOMContentLoaded', function() { 
  var calendarEl = document.getElementById('calendar'); 

  var calendar = new FullCalendar.Calendar(calendarEl, {
 
    selectable: true,
 
    }  
    );

  calendar.render();
  
});

//function holidays(){
  //var holidays = ["2019-02-18","2019-04-19","2019-05-20","2019-07-01","2019-08-05","2019-12-24"] 
  //var dt = datepicker.formatDate("yyyy-mm-dd");
//if (holidays == dt ){
  //holidays
//}
 
//}

