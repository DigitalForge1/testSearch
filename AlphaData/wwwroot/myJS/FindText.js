//поиск по странице

var lastResFind = ""; // последний удачный результат
var lastResFindEls = new Array(); // последний удачный результат
var copy_page = ""; // копия страницы в ихсодном виде
var numberFind = 1; // номер последнего найденого
var countFind = 1;
function TrimStr(s) {
    s = s.replace(/^\s+/g, '');
    return s.replace(/\s+$/g, '');
}
function FindOnPage(textToFind) {//ищет текст на странице

    if (textToFind == "") {
        alert("Вы ничего не ввели");
        return;
    }

    if (textToFind.length < 2) {
        alert("Слишком короткий текст")
        return;
    }

    //if (copy_page.length > 0)
    //    document.body.innerHTML = copy_page;
    //else copy_page = document.body.innerHTML;

    //document.querySelectorAll('a, td').forEach(el => {
    //    if (el.textContent.includes(textToFind))
    //        el.innerHTML = "<a name=" + textToFind + " style='background:yellow; font-size:15px'>" + el.innerHTML + "</a>" //.replace("/" + textToFind + "/gi", "<a name=" + textToFind + " style='background:yellow; font-size:15px'>");
    //})


    if (lastResFind == textToFind) {
        numberFind += 1;
        window.location = '#' + textToFind + numberFind;//перемещаем скрол к последнему найденному совпадению
        document.getElementById('main__content').scrollBy(0, -80); // подниаем в видимую область 
        if (countFind == numberFind) {
            numberFind = 0;
        }
        return;
    }
    else {
        numberFind = 1;
    }

    lastResFindEls.forEach(el => {
        ReturnOld(lastResFind, el);
    });
    regex = new RegExp(textToFind, 'gi');
    let i = 1;
    document.querySelectorAll('a, td').forEach(el => {
        if (el.textContent.includes(textToFind)) {
            let replace = el.innerHTML.replace(regex, "<a name=" + textToFind + i +" style='background:yellow; font-size:15px'>" + textToFind + "</a>");
            i++;
            if (replace != el.innerHTML) {
                lastResFindEls.push(el);
                lastResFind = textToFind;
                el.innerHTML = replace;
            }
        }
    });
    countFind = i;

    lastResFind = textToFind; // сохраняем фразу для поиска, чтобы в дальнейшем по ней стереть все ссылки
    window.location = '#' + textToFind;//перемещаем скрол к последнему найденному совпадению
    document.getElementById('main__content').scrollBy(0, -80);

}

function ReturnOld(lastResFind, lastResFindEl) {
    let r = '<a name="' + lastResFind + '.?.?' + '" style="background:yellow; font-size:15px">' + lastResFind + '<\/a>';
    let regex = new RegExp(r, 'gi');
    if (lastResFind != '') {
        lastResFindEl.innerHTML = lastResFindEl.innerHTML.replace(regex, lastResFind);
    }
}