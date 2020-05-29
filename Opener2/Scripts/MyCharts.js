// Набор данных для диаграммы.
class ChartDataset {
    constructor(options) {
        this.label = options.Label;
        this.borderColor = options.BorderColor;
        this.fill = false;
        this.data = options.Data;
    }
}

//Генератор наборов данных ChartDataset для диаграмм.
/*Конструктор должен принимать json типа:
{
    borderColor: "red",
        data: [{ x: "2020-05-18T00:00:00", y: "1" }, { x: "2020-05-19T00:00:00", y: "2" }, ...],
        fill: false,
        label: "label"
}
*/
class ChartDatasetGenerator {
    constructor(dataArray) {
        this.dataArray = dataArray;
    }

    generateDatasets() {
        var datasets = new Array();
        for (var index = 0; index < this.dataArray.length; index++) {
            var d = new ChartDataset(this.dataArray[index]);
            datasets[index] = d;
        }
        return datasets;
    }
}

//Диаграмма по датам.
//datasets создаются с помощью ChartDatasetGenerator
class DateChart {
    constructor(chartType, datasets, canvasId, minDate, maxDate) {
        this.chartType = chartType;
        this.datasets = datasets;
        this.canvasId = canvasId;
        this.minDate = minDate;
        this.maxDate = maxDate;
    }

    _checkDates() {
        console.log(this.minDate, this.maxDate);
        if (typeof this.minDate == "undefined") {
            this.minDate = new Date();
            this.minDate.setDate(this.minDate.getDate() - 30);
        }
        if (typeof this.maxDate == "undefined") {
            this.maxDate = new Date();
        }
    }

    createChart() {
        this._checkDates();
        var context = document.querySelector(this.canvasId).getContext("2d");
        var config = {
            type: this.chartType,
            data: {
                datasets: this.datasets
            },
            options: {
                responsive: true,
                title: {
                    display: true,
                    text: "Test chart"
                },
                //tooltips: {
                //    mode: 'index',
                //    intersect: false,
                //},
                legend: { display: true },
                scales: {
                    xAxes: [{
                        type: "time",
                        ticks: {
                            min: this.minDate,
                            max: this.maxDate,
                            unit: "day"
                        }
                    }]
                }
            }
        };
        var chart = new Chart(context, config);
        return chart;
    }
}