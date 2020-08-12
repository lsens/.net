function func() {

    //获取被选中的option标签 

    var vs = $('select  option:selected').val();
    var vs2 = $('#selectid2').val();
    var vs3 = $('#selectid2 option:selected').text();
    var myChart = echarts.init(document.getElementById('main1'));
    var arr = [18, 19, 20, 21, 22, 23, 0, 1, 2, 3, 4];
    
    $.ajax({
        type: "POST",
        url: "http://localhost:5000/api/User/GetData",
        contentType: "application/json",
        data: JSON.stringify({ "data": vs, "status": 0}),
        dataType: "json",
        success: function (msg) {

            var xData = $.map(msg.lineData, function (a) {
                var index = $.map(arr, function (n, i) {
                    return n == a ? i : null;
                });
                return index;
            });
            
            //  入睡时间格式 
            var option1 = {
                title: {
                    text: vs + '入睡时间趋势线',
                    left: "center"
                },
                tooltip: {
                    trigger: 'item',
                    formatter: function (params) {

                        var index = $.map(arr, function (n, i) {
                            return i == params.value ? n : null;
                        });

                        var color = params.color;//图例颜色
                        var htmlStr = '<div>';
                        htmlStr += params.name + '<br/>';//x轴的名称

                        //为了保证和原来的效果一样，这里自己实现了一个点的效果
                        htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                        //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                        htmlStr += params.seriesName + '为' + index + '点';

                        htmlStr += '</div>';

                        return htmlStr;

                    }
                },
                legend: {
                    data: ['入睡时间'],
                    left: "right"
                },
                xAxis: {
                    axisTick: {
                        show: false, //是否显示
                    },
                    axisLabel: {
                        rotate: -15,
                        interval: 1
                    },
                    data: msg.lineTime
                },
                yAxis: {
                    axisTick: {
                        show: false, //是否显示
                    },
                    axisLabel: {
                        rotate: 0,
                        interval: 1
                    },
                    data: arr
                },
                series: [{
                    name: '入睡时间',
                    type: 'line',
                    data: xData,
                }]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart.setOption(option1);
            $("#stime").text(msg.lineTime[0].split(",")[0])
            $("#etime").text(msg.lineTime.pop().split(",").pop())
            $("#time1").text(msg.normalData[0])
            $("#time2").text(msg.normalData[1])
            $("#time3").text(msg.pointData)
        }
    })

    var myChart2 = echarts.init(document.getElementById('main2'));
    $.ajax({
        type: "POST",
        url: "http://localhost:5000/api/User/GetData",
        contentType: "application/json",
        data: JSON.stringify({ "data": vs, "status": parseInt(vs2) }),
        dataType: "json",
        success: function (msg) {
            console.log(msg);

            var points = [];
            for (var i = 0; i < msg.pointData.length; i++) {
                points.push(
                    [msg.pointTime[i], msg.pointData[i]]
                )
            }

            //  趋势线格式
            var option2 = {
                title: {
                    text: vs + vs3 + '趋势线',
                    left: "center"
                },
                tooltip: {
                    trigger: 'item',
                    formatter: function (params) {
                        if (params.seriesName == vs3 + '次数') {
                            var color = params.color;//图例颜色
                            var htmlStr = '<div>';
                            htmlStr += params.name + '<br/>';//x轴的名称

                            //为了保证和原来的效果一样，这里自己实现了一个点的效果
                            htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                            //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                            htmlStr += params.seriesName + '为' + params.value + '次';

                            htmlStr += '</div>';

                            return htmlStr;
                        }
                        if (params.seriesName == vs3 + '变化次数') {
                            var color = params.color;//图例颜色
                            var htmlStr = '<div>';
                            htmlStr += params.name + '<br/>';//x轴的名称

                            //为了保证和原来的效果一样，这里自己实现了一个点的效果
                            htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                            //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                            htmlStr += vs3 + '变化幅度为' + params.value[1] + '次';

                            htmlStr += '</div>';

                            return htmlStr;
                        }
                    }
                },
                legend: {
                    orient: "vertical", 
                    align: 'left',
                    width: 500,
                    data: [vs3 + '次数', vs3 + '变化次数'],
                    itemWidth: 8,//图例图标宽度
                    itemHeight: 8,//图例图标高度
                    left: "right"
                },
                xAxis: {
                    axisTick: {
                        show: false, //是否显示
                    },
                    axisLabel: {
                        rotate: -15,
                        interval: 1
                    },
                    data: msg.lineTime
                },
                yAxis: {
                },
                series: [{
                    name: vs3 + '次数',
                    type: 'line',
                    data: msg.lineData,
                    smooth: true
                },
                    {
                        name: vs3 + '变化次数',
                        symbol: "triangle",
                        symbolSize: 10,
                        data: points,
                        type: 'scatter'
                    }
                ]
            };

            var normalData2 = msg.normalData[1]

            if (msg.normalData[0] == msg.normalData[1]) {
                normalData2 = msg.normalData[0] + 1;
            }

            // 使用刚指定的配置项和数据显示图表。
            myChart2.setOption(option2);
            $("#stime1").text(msg.pointTime[0])
            $("#etime1").text(msg.pointTime[1])
            $("#data1").text(msg.pointData[0])
            $("#data2").text(msg.pointData[1])
            $("#vs3").text(vs3)
            $("#normalData1").text(msg.normalData[0])
            $("#normalData2").text(normalData2)
            $("#main4").show();

        }
    })

    var myChart3 = echarts.init(document.getElementById('main3'));
    var week = ["周一", "周二到周四", "周五", "周末", "节假日"]
    $.ajax({
        type: "POST",
        url: "http://localhost:5000/api/User/GetData",
        contentType: "application/json",
        data: JSON.stringify({ "data": vs, "status": 90 + parseInt(vs2) }),
        dataType: "json",
        success: function (msg) {

            console.log(msg);

            var option = {
                title: {
                    text: '日期分析模型',
                    left: "center"
                },
                tooltip: {
                    trigger: 'item',
                    formatter: function (params) {
                        var color = params.color;//图例颜色
                        var htmlStr = '<div>';
                        htmlStr += "在" + params.name + '<br/>';//x轴的名称

                        //为了保证和原来的效果一样，这里自己实现了一个点的效果
                        htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                        //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                        htmlStr += "出现" + vs3 + '的情况为' + params.value + '天';

                        htmlStr += '</div>';

                        return htmlStr;
                    }
                },
                legend: {
                    data: [vs3 + '天数'],
                    left: "right"
                },
                xAxis: {
                    axisTick: {
                        show: false, //是否显示
                    },
                    axisLabel: {
                        rotate: 0,
                        interval: 0
                    },
                    data: week
                },
                yAxis: {
                },
                series: [{
                    name: vs3 + '次数',
                    type: 'bar',
                    data: msg.weekCount
                }]
            };

            // 使用刚指定的配置项和数据显示图表。
            myChart3.setOption(option);
        }
    })

    var myChart5 = echarts.init(document.getElementById('main6'));
    var myChart6 = echarts.init(document.getElementById('main7'));
    myChart5.dispose();
    myChart6.dispose();

}  

function func2() {

    var vs = $('select  option:selected').val();
    var vs2 = $('#selectid2').val();
    var vs3 = $('#selectid2 option:selected').text();
    var vs4 = $('#selectid3').val();
    var vs5 = $('#selectid3 option:selected').text();

    var myChart4 = echarts.init(document.getElementById('main7'));
    myChart4.dispose();

    var myChart4 = echarts.init(document.getElementById('main5'));
    $.ajax({
        type: "POST",
        url: "http://localhost:5000/api/User/GetData",
        contentType: "application/json",
        data: JSON.stringify({ "data": vs, "status": 200 + parseInt(vs2), "month": parseInt(vs4)}),
        dataType: "json",
        success: function (msg) {
            console.log(msg);

            //var xInterval;
            //对所有数据的x轴坐标进行省略
            //if (parseInt(vs4) == 13) {
            //    xInterval = 10;
            //} else {
            //    xInterval = 0;
            //};

            // 循环本次数据取最大值 判断颜色的渐变  

            var maxPointData = Math.max.apply(null, msg.pointData);
            var offset0 = 0;
            var offset1 = 0;
            var offset2 = 1;
            var color1 = 'pink';

            // 面积颜色规定
            if (maxPointData > 50) {
                offset1 = 1 - 20 / maxPointData;
            } else if (maxPointData < 10) {
                color1 = 'green';
            } 

            var colorArray = [
                { offset: offset0, color: 'red' },
                { offset: offset1, color: color1 },
                { offset: offset2, color: 'green' }
            ];

            // 映射获取日期中的天

            var lineTimeDay = msg.lineTime.map(a => a.slice(8, 10));

            var option = {
                title: {
                    text: vs + vs5 + vs3 + "趋势线",
                    left: "center"
                },
                tooltip: {
                    trigger: 'item',
                    formatter: function (params) {
                        var data;
                        for (var i = 0; i < msg.lineTime.length; i++) {
                            if (msg.lineTime[i] == params.name) {
                                data = msg.pointData[i];
                            }
                        }
                        var color = params.color;//图例颜色
                        var htmlStr = '<div>';
                        htmlStr += "在" + vs5 + params.name + '号' + '<br/>';//x轴的名称

                        //为了保证和原来的效果一样，这里自己实现了一个点的效果
                        htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                        //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                        htmlStr += "出现" + vs3 + '的情况为' + data + '次';

                        htmlStr += '</div>';

                        return htmlStr;

                    }
                },
                xAxis: {
                    axisTick: {
                        show: false, //是否显示
                    },
                    axisLabel: {
                        rotate: 0,
                        interval: 0
                    },
                    data: lineTimeDay
                },
                yAxis: {
                },
                //dataZoom: [//缩放
                //    {
                //        type: 'slider',//对应第一个y轴
                //        show: false,
                //        start: 0,//%
                //        end: 100,
                //        // handleSize: 8
                //    },
                //    {
                //        type: 'inside',//区域缩放
                //        start: 0,
                //        end: 100
                //    },
                //],
                series: [{
                    name: vs3 + '次数',
                    //type: 'bar',
                    type: 'line',
                    data: msg.lineData,
                    symbol: "circle",
                    areaStyle: {
                        normal: {
                            color: new echarts.graphic.LinearGradient(
                                0, 0, 0, 1,
                                colorArray
                            )
                        }
                    },
                    barMaxWidth: 40,
                    smooth: true,
                    lineStyle: {
                        normal: {
                            color: 'red',
                            width: 1
                        }
                    },
                    itemStyle: {
                        normal: {
                            color: function (param) { //拐点颜色
                                var itemColor = 'green';
                                for (var i = 0; i < msg.abPointTime.length; i++) {
                                    if (param.name == msg.abPointTime[i].slice(8,10)) {
                                        itemColor = 'red';
                                    }
                                }
                                return itemColor
                            },
                        }
                    },
                    symbolSize: 8
                }]
            };

            myChart4.setOption(option);

            var myChart5 = echarts.init(document.getElementById('main6'));
            var myChart6 = echarts.init(document.getElementById('main7'));
            myChart5.dispose();
            myChart6.dispose();

            // 拐点点击事件 
            myChart4.on('click', function (params) {
                console.log(option.title.text)

                var myChart5 = echarts.init(document.getElementById('main6'));
                var myChart6 = echarts.init(document.getElementById('main7'));
                myChart5.dispose();
                myChart6.dispose();

                var myChart5 = echarts.init(document.getElementById('main6'));
                var myChart6 = echarts.init(document.getElementById('main7'));

                if (params.value > 0) {
                    $.ajax({
                        type: "POST",
                        url: "http://localhost:5000/api/User/GetData",
                        contentType: "application/json",
                        data: JSON.stringify({ "data": vs, "status": 101, "month": parseInt(vs4), "day": parseInt(params.name) }),
                        dataType: "json",
                        success: function (msg) {
                            console.log(msg)
                            var option = {
                                title: {
                                    text: vs5 + params.name + '日心率异常趋势线',
                                    left: "center"
                                },
                                tooltip: {
                                    trigger: 'item',
                                    formatter: function (params) {
                                        var data;
                                        for (var i = 0; i < msg.lineTime.length; i++) {
                                            if (msg.lineTime[i] == params.name) {
                                                data = msg.pointData[i];
                                            }
                                        }
                                        var color = params.color;//图例颜色
                                        var htmlStr = '<div>';
                                        htmlStr += "在" + params.name + '后半小时内' + '<br/>';//x轴的名称

                                        //为了保证和原来的效果一样，这里自己实现了一个点的效果
                                        htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                                        //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                                        htmlStr += '出现心率异常的情况为' + data + '次';

                                        htmlStr += '</div>';

                                        return htmlStr;
                                    }
                                },

                                xAxis: {
                                    axisTick: {
                                        show: false, //是否显示
                                    },
                                    axisLabel: {
                                        rotate: -15,
                                        interval: 1
                                    },
                                    data: msg.lineTime
                                },
                                yAxis: {
                                },
                                series: {
                                    name: '心率异常次数',
                                    type: 'line',
                                    data: msg.lineData,
                                    smooth: true
                                }
                            };
                            myChart5.setOption(option);
                        }
                    })

                    $.ajax({
                        type: "POST",
                        url: "http://localhost:5000/api/User/GetData",
                        contentType: "application/json",
                        data: JSON.stringify({ "data": vs, "status": 102, "month": parseInt(vs4), "day": parseInt(params.name) }),
                        dataType: "json",
                        success: function (msg) {
                            console.log(msg.pointData)
                            var option = {
                                title: {
                                    text: vs5 + params.name + '日呼吸异常趋势线',
                                    left: "center"
                                },
                                tooltip: {
                                    trigger: 'item',
                                    formatter: function (params) {
                                        var data;
                                        for (var i = 0; i < msg.lineTime.length; i++) {
                                            if (msg.lineTime[i] == params.name) {
                                                data = msg.pointData[i];
                                            }
                                        }
                                        var color = params.color;//图例颜色
                                        var htmlStr = '<div>';
                                        htmlStr += "在" + params.name + '后半小时内' + '<br/>';//x轴的名称

                                        //为了保证和原来的效果一样，这里自己实现了一个点的效果
                                        htmlStr += '<span style="margin-right:5px;display:inline-block;width:10px;height:10px;border-radius:5px;background-color:' + color + ';"></span>';

                                        //添加一个汉字，这里你可以格式你的数字或者自定义文本内容
                                        htmlStr += "出现呼吸异常" + '的情况为' + data + '次';

                                        htmlStr += '</div>';

                                        return htmlStr;
                                    }
                                },

                                xAxis: {
                                    axisTick: {
                                        show: false, //是否显示
                                    },
                                    axisLabel: {
                                        rotate: -15,
                                        interval: 1
                                    },
                                    data: msg.lineTime
                                },
                                yAxis: {
                                },
                                series: {
                                    name: '呼吸异常次数',
                                    type: 'line',
                                    data: msg.lineData,
                                    smooth: true
                                }
                            };
                            myChart6.setOption(option);
                        }
                    })
                }

            });
        }
    })

}


