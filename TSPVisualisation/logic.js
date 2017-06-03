
var rad = 10;

var height = $('#canvas').height();
var width = $('#canvas').width();

var force = d3.layout.force()
        .size([width, height])
        .on("tick", tick);

var svg = d3.select('#canvas');

var d3nodes = force.nodes();
var d3links = force.links();
var svgnodes = svg.selectAll(".node");
var svglinks = svg.selectAll(".link");

var started = false;

var speed = 170;
var curMethod = 'greedy';


function select_method(e, name)
{
    if (!started) {
        $('.selected-method').removeClass('selected-method');
        $(e).children().addClass('selected-method');
        curMethod = name;
    }
}

function change_duration()
{
    speed = Math.abs(700 - +$("#duration-inp").val()) + 70;
}

$("#canvas").click(function (e)
{
    var xClick = e.pageX - $(this).offset().left;
    var yClick = e.pageY - $(this).offset().top;

    add_node(xClick, yClick);
});

function add_random()
{
    var toAdd = $('#random-to-add').val();
    for (var i = 0, j = 0; i < 100 && j < +toAdd; ++i) {
        if (add_node(getRand(0, width), getRand(0, height))) {
            ++j;
        }
    }
}

function add_node(x, y)
{
    if (started || d3nodes.length > 50 || x <= 15 || y <= 15 || width - x <= 15 || height - y <= 15) {
        return false;
    }
    var toAdd = {x: x, y: y, fixed: true};
    for (var i = 0; i < d3nodes.length; ++i) {
        if (dist(d3nodes[i], toAdd) <= rad * 5) {
            return false;
        }
    }
    d3nodes.push(toAdd);
    for (var i = 0; i < d3nodes.length; ++i) {
        d3links.push({source: toAdd, target: d3nodes[i], cost: Math.round(dist(d3nodes[i], toAdd))});
    }
    restart();

    return true;
}

function delete_node(el, pos)
{
    if (started) {
        return;
    }

    d3nodes.splice(pos, 1);
    d3links = d3links.filter(function (link) {
        return link.source !== el && link.target !== el;
    });
    d3.event.stopPropagation();

    restart();
}

var breaked = false;

function clear_all()
{
    breaked = true;

    setTimeout(function () {
        started = false;
        vis_queu = [];
        visi = 0;
        curDel = 0;
        lastfront = true;
        initForce();
    }, speed * 1.1);
}

function getRand(min, max) {
    return Math.random() * (max - min) + min;
}

function dist(node1, node2)
{
    return Math.sqrt(Math.pow(node1.x - node2.x, 2) + Math.pow(node1.y - node2.y, 2));
}



function initForce() 
{
    svg.selectAll('*').remove();

    force = d3.layout.force()
        .size([width, height])
        .on("tick", tick);

    d3nodes = force.nodes();
    d3links = force.links();
    svgnodes = svg.selectAll(".node");
    svglinks = svg.selectAll(".link");

    force.start();
}

function tick() {
    svglinks.attr("x1", function (d) { return d.source.x; })
        .attr("y1", function (d) { return d.source.y; })
        .attr("x2", function (d) { return d.target.x; })
        .attr("y2", function (d) { return d.target.y; });

    svgnodes.attr("cx", function (d) { return d.x; })
        .attr("cy", function (d) { return d.y; });
}

function restart() {
    svgnodes = svgnodes.data(d3nodes);

    svgnodes.enter().insert("circle")
        .attr("class", "node")
        .attr("r", rad)
        .on("click", delete_node);

    svgnodes.exit()
        .remove();

    svglinks = svglinks.data(d3links);

    svglinks.enter().insert("line", ".node")
        .attr("class", "link");

    svglinks.exit()
        .remove();

    force.start();
}




var vis_queu = [];
var visi = 0;
var curDel = 0;
var lastfront = true;

function play() 
{
    breaked = false;
    step_front(true);
}

function play_back()
{
    breaked = false;
    step_back(true);
}

function step_back_click()
{
    breaked = false;
    step_back(false);
}

function step_front_click() {
    breaked = false;
    step_front(false);
}

function step_back(toend)
{
    if (breaked) return;
    if (started) {
        if (lastfront) {
            --visi;
            lastfront = false;
        }
        while (visi > 0) {

            reverse(vis_queu[visi].type)(vis_queu[visi].from, vis_queu[visi].to, vis_queu[visi].color_stroke, vis_queu[visi].color_fill, curDel);
            --visi;

            if (vis_queu[visi].no_delay !== true) {
                curDel += speed;
                setTimeout(function () {
                    curDel -= speed;
                }, curDel);

                break;
            }
        }

        if (toend === true && visi > 0) {
            setTimeout(function () {
                step_back(true);
            }, speed);
        }
    }
}

function step_front(toend)
{
    if (breaked) return;
    if (!started) {
        pre_count();
        started = true;
    }
    if (!lastfront) {
        ++visi;
        lastfront = true;
    }

    while (visi != vis_queu.length) {

        vis_queu[visi].type(vis_queu[visi].from, vis_queu[visi].to, vis_queu[visi].color_stroke, vis_queu[visi].color_fill, curDel);

        if (vis_queu[visi].no_delay !== true) {
            curDel += speed;
            setTimeout(function () {
                curDel -= speed;
            }, curDel);

            ++visi;
            break;
        }
        ++visi;
    }

    if (toend === true && visi != vis_queu.length) {
        setTimeout(function () {
            step_front(true);
        }, speed);
    }
}



function reverse(func)
{
    if (func === show_link) {
        return hide_link;
    } else if (func === hide_link) {
        return show_link;
    } else if (func === show_node) {
        return hide_node;
    } else if (func == hide_node) {
        return show_node;
    }
}

function show_link(from, to, color_stroke, color_fill_DUMMY, del)
{
    if (del == undefined) del = 0;
    var link = svglinks.filter(function (link) {
        return (link.source === d3nodes[from] && link.target === d3nodes[to]) || (link.target === d3nodes[from] && link.source === d3nodes[to]);
    });

    link.transition().ease('linear').duration(speed).delay(del).style("stroke-width", 3).style("stroke", color_stroke);
}

function hide_link(from, to, color_stroke_DUMMY, color_fill_DUMMY, del)
{
    if (del == undefined) del = 0;
    var link = svglinks.filter(function (link) {
        return (link.source === d3nodes[from] && link.target === d3nodes[to]) || (link.target === d3nodes[from] && link.source === d3nodes[to]);
    });

    link.transition().ease('linear').duration(speed).delay(del).style("stroke-width", 0);
}

function show_node(from, to_DUMMY, color_stroke, color_fill, del)
{
    if (del == undefined) del = 0;
    if (color_stroke == undefined) color_stroke = default_color;
    if (color_fill == undefined) color_fill = none_color;

    var node = svgnodes.filter(function (node) {
        return node === d3nodes[from];
    });

    node.transition().ease('linear').duration(speed).delay(del)
        .style("stroke", color_stroke)
        .style("fill", color_fill);
}

function hide_node(from, to_DUMMY, color_stroke, color_fill, del) {
    if (del == undefined) del = 0;

    var node = svgnodes.filter(function (node) {
        return node === d3nodes[from];
    });

    node.transition().ease('linear').duration(speed).delay(del)
        .style("stroke", default_color)
        .style("fill", none_color);
}

function pre_count()
{
    if (curMethod === "greedy") {
        greedy_precount();
    } else if (curMethod === "dynamic") {
        dynamic_precount();
    } else if (curMethod === "gradient") {
        gradient_precount(false);
    } else if (curMethod === "simulation") {
        gradient_precount(true);
    } else if (curMethod === "two-opt") {
        two_opt_precount();
    } else if (curMethod === "random") {
        random_precount();
    }
}


var default_color = "#cccccc",
    none_color = "#ffffff",
    question_color = "#336699",
    selected_color = "#339933",
    checking_color = "#FF9933",
    remember_color = "#CC3333";

var inf = 10000000;

function length(from, to)
{
    if (from === to) {
        return inf + 20;
    }
    return d3links.filter(function (link) {
        return (link.source === d3nodes[from] && link.target === d3nodes[to]) || (link.target === d3nodes[from] && link.source === d3nodes[to]);
    })[0].cost;
}

function greedy_precount()
{
    var used = [];
    for (var i = 0; i < d3nodes.length; ++i) {
        used[i] = false;
    }
    var path = [];
    var start = 0;
    var cur = start;
    path.push(cur);

    vis_queu.push({ type: show_node, from: cur, color_stroke: question_color });

    for (var i = 0; i < d3nodes.length - 1; ++i) {
        var toNode = -1, minlen = inf;
        for (var j = 0; j < d3nodes.length; ++j) {
            if (cur != j && used[j] === false) {

                vis_queu.push({ type: show_link, from: cur, to: j, color_stroke: checking_color });

                if (minlen > length(cur, j)) {
                    
                    if (toNode != -1) vis_queu.push({ type: hide_node, from: toNode, color_stroke: remember_color });
                    vis_queu.push({ type: show_node, from: j, color_stroke: remember_color });

                    minlen = length(cur, j);
                    toNode = j;
                }

                vis_queu.push({ type: hide_link, from: cur, to: j, color_stroke: checking_color });
            }
        }

        vis_queu.push({ type: show_link, from: cur, to: toNode, color_stroke: question_color });
        vis_queu.push({ type: hide_node, from: toNode, color_stroke: remember_color });
        vis_queu.push({ type: show_node, from: toNode, color_stroke: question_color });

        used[cur] = true;
        cur = toNode;
        path.push(cur);
    }
    vis_queu.push({ type: show_link, from: cur, to: start, color_stroke: question_color });

    hilite_path(path, question_color, selected_color);
}

function random_precount(not_end)
{
    var used = [];
    for (var i = 0; i < d3nodes.length; ++i) {
        used[i] = false;
    }
    var path = [];
    var start = 0;
    var cur = start;
    path.push(cur);
    used[start] = true;

    vis_queu.push({ type: show_node, from: cur, color_stroke: question_color });

    for (var i = 0; i < d3nodes.length - 1; ++i) {
        var toNode = -1;
        while (true) {
            var j = Math.round(getRand(0, d3nodes.length - 1));
            if (cur != j && used[j] === false) {

                toNode = j;
                break;
            }
        }

        vis_queu.push({ type: show_link, from: cur, to: toNode, color_stroke: question_color });
        vis_queu.push({ type: show_node, from: toNode, color_stroke: question_color });

        cur = toNode;
        used[cur] = true;
        path.push(cur);
    }
    vis_queu.push({ type: show_link, from: cur, to: start, color_stroke: question_color });

    if (not_end !== true) {
        hilite_path(path, question_color, selected_color);
    }
    return path;
}

function hilite_path(path, from_color, to_color)
{
    for (var i = 0; i < path.length - 1; ++i) {
        vis_queu.push({ type: hide_node, from: path[i], color_stroke: from_color, no_delay: true });
        vis_queu.push({ type: hide_link, from: path[i], to: path[i + 1], color_stroke: from_color, no_delay: true });
    }
    vis_queu.push({ type: hide_node, from: path[path.length - 1], color_stroke: from_color, no_delay: true });
    vis_queu.push({ type: hide_link, from: path[path.length - 1], to: path[0], color_stroke: from_color });

    for (var i = 0; i < path.length - 1; ++i) {
        vis_queu.push({ type: show_node, from: path[i], color_stroke: to_color, no_delay: true });
        vis_queu.push({ type: show_link, from: path[i], to: path[i + 1], color_stroke: to_color, no_delay: true });
    }
    vis_queu.push({ type: show_node, from: path[path.length - 1], to: path[0], color_stroke: to_color, no_delay: true });
    vis_queu.push({ type: show_link, from: path[path.length - 1], color_stroke: to_color, to: path[0] });
}

function show_path(path, to_color)
{
    for (var i = 0; i < path.length - 1; ++i) {
        vis_queu.push({ type: show_node, from: path[i], color_stroke: to_color, no_delay: true });
        vis_queu.push({ type: show_link, from: path[i], to: path[i + 1], color_stroke: to_color, no_delay: true });
    }
}

function hide_path(path, from_color) {
    for (var i = 0; i < path.length - 1; ++i) {
        vis_queu.push({ type: hide_node, from: path[i], color_stroke: from_color, no_delay: true });
        vis_queu.push({ type: hide_link, from: path[i], to: path[i + 1], color_stroke: from_color, no_delay: true });
    }
}

function copyp(path)
{
    var np = [];
    for (var i = 0; i < path.length; ++i) {
        np.push(path[i]);
    }
    return np;
}

function dynamic_precount()
{
    if (d3nodes.length > 8) {
        started = false;
        return;
    }

    var dp = [];
    for (var mask = 0; mask < 1 << d3nodes.length; ++mask) {
        dp[mask] = [];
        for (var i = 0; i < d3nodes.length; ++i) {
            dp[mask][i] = {};
            dp[mask][i].weight = inf + 5;
            dp[mask][i].path = [];
        }
    }
    dp[0][0].weight = 0;
    dp[0][0].path.push(0);

    vis_queu.push({type: show_node, from: 0, color_stroke: checking_color});

    for (var nextNode = 1; nextNode < d3nodes.length; ++nextNode) {

        dp[1 << nextNode][nextNode].weight = length(0, nextNode);
        dp[1 << nextNode][nextNode].path = [0, nextNode];

        vis_queu.push({ type: show_link, from: 0, to: nextNode, color_stroke: checking_color, no_delay: true });

        vis_queu.push({ type: show_node, from: nextNode, color_stroke: checking_color });

        vis_queu.push({ type: hide_node, from: nextNode, color_stroke: checking_color, no_delay: true });
        vis_queu.push({ type: hide_link, from: 0, to: nextNode, color_stroke: checking_color, no_delay: true });

        vis_queu.push({ type: hide_node, from: 0, color_stroke: checking_color, no_delay: true });
        vis_queu.push({ type: show_node, from: 0, color_stroke: selected_color, no_delay: true });

        vis_queu.push({ type: show_link, from: 0, to: nextNode, color_stroke: selected_color, no_delay: true });
        vis_queu.push({ type: show_node, from: nextNode, color_stroke: selected_color });


        vis_queu.push({ type: hide_node, from: 0, color_stroke: selected_color, no_delay: true });
        vis_queu.push({ type: show_node, from: 0, color_stroke: checking_color, no_delay: true });

        vis_queu.push({ type: hide_node, from: nextNode, color_stroke: selected_color, no_delay: true });
        vis_queu.push({ type: hide_link, from: 0, to: nextNode, color_stroke: checking_color });
    }

    vis_queu.push({ type: hide_node, from: 0, color_stroke: checking_color });

    for (var mask = 0; mask < 1 << d3nodes.length; ++mask) {

        for (var i = 1; i < d3nodes.length; ++i) {

            if ((mask & (1 << i)) != 0 && dp[mask][i].path[0] == 0) {

                show_path(dp[mask][i].path, question_color);
                vis_queu.push({ type: show_node, from: i, color_stroke: checking_color });

                for (var nextNode = 0; nextNode < d3nodes.length; ++nextNode) {

                    var len = length(i, nextNode);

                    if ((mask & (1 << nextNode)) == 0 && 
                        (nextNode != 0 || ((mask | (1 << nextNode)) == ((1 << d3nodes.length) - 1)))) {


                        vis_queu.push({ type: show_link, from: i, to: nextNode, color_stroke: checking_color, no_delay: true });
                        vis_queu.push({ type: show_node, from: nextNode, color_stroke: checking_color });


                        var toMask = mask | (1 << nextNode);

                        if (dp[toMask][nextNode].weight > Math.min(dp[mask][i].weight + len, inf)) {

                            hide_path(dp[mask][i].path, question_color);
                            show_path(dp[mask][i].path, selected_color);

                            vis_queu.push({ type: hide_node, from: i, color_stroke: checking_color, no_delay: true });
                            vis_queu.push({ type: show_node, from: i, color_stroke: selected_color, no_delay: true });

                            vis_queu.push({ type: hide_node, from: nextNode, color_stroke: checking_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: i, to: nextNode, color_stroke: checking_color, no_delay: true });

                            vis_queu.push({ type: show_link, from: i, to: nextNode, color_stroke: selected_color, no_delay: true });
                            vis_queu.push({ type: show_node, from: nextNode, color_stroke: selected_color });



                            hide_path(dp[mask][i].path, selected_color);
                            show_path(dp[mask][i].path, question_color);

                            vis_queu.push({ type: hide_node, from: i, color_stroke: selected_color, no_delay: true });
                            vis_queu.push({ type: show_node, from: i, color_stroke: checking_color, no_delay: true });

                            vis_queu.push({ type: hide_node, from: nextNode, color_stroke: selected_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: i, to: nextNode, color_stroke: checking_color });



                            dp[toMask][nextNode].weight = Math.min(dp[mask][i].weight + len, inf);

                            dp[toMask][nextNode].path = copyp(dp[mask][i].path);
                            dp[toMask][nextNode].path.push(nextNode);

                        } else {
                            vis_queu.push({ type: hide_node, from: nextNode, color_stroke: checking_color });
                            vis_queu.push({ type: hide_link, from: i, to: nextNode, color_stroke: checking_color });
                        }
                    }
                }

                hide_path(dp[mask][i].path, question_color);
                vis_queu.push({ type: hide_node, from: i, color_stroke: checking_color });

            }
        }
    }

    show_path(dp[(1 << d3nodes.length) - 1][0].path, selected_color);
    vis_queu.push({ type: show_node, from: 0, color_stroke: selected_color });

}


function gradient_precount(showAll)
{
    var path = random_precount(true);
    hilite_path(path, question_color, default_color);

    var found = true;
    while (found) {
        found = false;
        for (var from1_i = 0; from1_i < d3nodes.length; ++from1_i) {
            for (var from2_i = 0; from2_i < d3nodes.length; ++from2_i) {
                var to1_i = (from1_i + 1) % d3nodes.length;
                var to2_i = (from2_i + 1) % d3nodes.length;

                var from1 = path[from1_i];
                var from2 = path[from2_i];
                var to1 = path[to1_i];
                var to2 = path[to2_i];

                if (from1 !== to2 && from1 !== from2 && from2 !== to1 && to1 !== to2) {

                    if (showAll === true) {
                        vis_queu.push({ type: hide_link, from: from1, to: to1, color_stroke: default_color, no_delay: true });
                        vis_queu.push({ type: hide_link, from: from2, to: to2, color_stroke: default_color, no_delay: true });


                        vis_queu.push({ type: show_link, from: from1, to: from2, color_stroke: checking_color, no_delay: true });
                        vis_queu.push({ type: show_link, from: to2, to: to1, color_stroke: checking_color, no_delay: true });

                        vis_queu.push({ type: show_link, from: from1, to: to1, color_stroke: question_color, no_delay: true });
                        vis_queu.push({ type: show_link, from: from2, to: to2, color_stroke: question_color });
                    }

                    if ((length(from1, to1) + length(from2, to2)) > (length(from1, from2) + length(to2, to1))) {

                        if (showAll === false) {
                            vis_queu.push({ type: hide_link, from: from1, to: to1, color_stroke: default_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: from2, to: to2, color_stroke: default_color, no_delay: true });


                            vis_queu.push({ type: show_link, from: from1, to: from2, color_stroke: checking_color, no_delay: true });
                            vis_queu.push({ type: show_link, from: to2, to: to1, color_stroke: checking_color, no_delay: true });

                            vis_queu.push({ type: show_link, from: from1, to: to1, color_stroke: question_color, no_delay: true });
                            vis_queu.push({ type: show_link, from: from2, to: to2, color_stroke: question_color });
                        }


                        vis_queu.push({ type: hide_link, from: from1, to: from2, color_stroke: checking_color, no_delay: true });
                        vis_queu.push({ type: hide_link, from: to2, to: to1, color_stroke: checking_color, no_delay: true });

                        vis_queu.push({ type: show_link, from: from1, to: from2, color_stroke: selected_color, no_delay: true });
                        vis_queu.push({ type: show_link, from: to2, to: to1, color_stroke: selected_color });


                        vis_queu.push({ type: hide_link, from: from1, to: to1, color_stroke: question_color, no_delay: true });
                        vis_queu.push({ type: hide_link, from: from2, to: to2, color_stroke: question_color });



                        vis_queu.push({ type: hide_link, from: from1, to: from2, color_stroke: selected_color, no_delay: true });
                        vis_queu.push({ type: hide_link, from: to2, to: to1, color_stroke: selected_color, no_delay: true });

                        vis_queu.push({ type: show_link, from: from1, to: from2, color_stroke: default_color, no_delay: true });
                        vis_queu.push({ type: show_link, from: to2, to: to1, color_stroke: default_color });


                        for (var i = to1_i, j = from2_i; i != j;) {
                            var temp = path[i];
                            path[i] = path[j];
                            path[j] = temp;

                            i = (i + 1) % path.length;
                            if (i == j) {
                                break;
                            }
                            --j;
                            if (j < 0) {
                                j = path.length - 1;
                            }
                        }

                        found = true;
                        break;

                    } else {
                        if (showAll === true) {
                            vis_queu.push({ type: hide_link, from: from1, to: to1, color_stroke: question_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: from2, to: to2, color_stroke: question_color, no_delay: true });

                            vis_queu.push({ type: show_link, from: from1, to: to1, color_stroke: selected_color, no_delay: true });
                            vis_queu.push({ type: show_link, from: from2, to: to2, color_stroke: selected_color });



                            vis_queu.push({ type: hide_link, from: from1, to: from2, color_stroke: checking_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: to2, to: to1, color_stroke: checking_color });



                            vis_queu.push({ type: hide_link, from: from1, to: to1, color_stroke: selected_color, no_delay: true });
                            vis_queu.push({ type: hide_link, from: from2, to: to2, color_stroke: selected_color, no_delay: true });

                            vis_queu.push({ type: show_link, from: from1, to: to1, color_stroke: default_color, no_delay: true });
                            vis_queu.push({ type: show_link, from: from2, to: to2, color_stroke: default_color });
                        }
                    }
                }
            }
        }
    }
    hilite_path(path, default_color, selected_color);
}

function two_opt_precount()
{
    var g = []; 
    var res = [];
    var tree_id = [];

    for (var i = 0; i < d3nodes.length; ++i) {
        tree_id[i] = i;
        for (var j = i + 1; j < d3nodes.length; ++j) {
            g.push({from: i, to: j, weight: length(i, j)})
        }
    }

    g = g.sort(function (a, b) { return a.weight - b.weight; });

    for (var i = 0; i < g.length; ++i) {

        var from = g[i].from, to = g[i].to, l = g[i].weight;

        if (tree_id[from] != tree_id[to]) {

            res.push({ from: from, to: to });

            vis_queu.push({ type: show_link, from: from, to: to, color_stroke: checking_color });

            var old_id = tree_id[to], new_id = tree_id[from];
            for (var j = 0; j < d3nodes.length; ++j) {
                if (tree_id[j] == old_id) {
                    tree_id[j] = new_id;
                }
            }
        }
    }
    var graph = [], was = [];
    for (var i = 0; i < d3nodes.length; ++i) {
        graph[i] = [];
        was[i] = false;
    }

    for (var i = 0; i < res.length; ++i) {
        graph[res[i].from].push(res[i].to);
        graph[res[i].to].push(res[i].from);
    }
    var path = [];

    dfs(0, [-1], graph, was, path);

    vis_queu.push({ type: show_link, from: path[0], to: path[path.length - 1], color_stroke: question_color });

    hilite_path(path, question_color, selected_color);
}

function dfs(cur, last, graph, was, path)
{
    was[cur] = true;
    path.push(cur);
    if (last[0] != -1) {
        vis_queu.push({ type: show_link, from: last[0], to: cur, color_stroke: question_color });
    }
    vis_queu.push({ type: show_node, from: cur, color_stroke: question_color });

    last[0] = cur;
    for (var i = 0; i < graph[cur].length; ++i) {

        if (was[graph[cur][i]]) continue;

        vis_queu.push({ type: hide_link, from: cur, to: graph[cur][i], color_stroke: checking_color });

        dfs(graph[cur][i], last, graph, was, path);
    }
}
