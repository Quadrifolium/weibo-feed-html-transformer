// ==UserScript==
// @name        Get HTML from Weibo
// @namespace   https://github.com/quadrifolium
// @description Get HTML of a Weibo feed for further usage.
// @author      Quadrifolium
// @version     0.0.1
// @match       http://weibo.com/*
// @match       https://weibo.com/*
// @match       http://www.weibo.com/*
// @match       https://www.weibo.com/*
// @grant       GM.setClipboard
// ==/UserScript==

var numberOfFeeds = 0;	// number of feeds in the current page

// Initialisation.
document.addEventListener('readystatechange', pageReadyActions);
//document.addEventListener('DOMContentLoaded', menuListListen);

// Add event listeners.
function pageReadyActions() {
  if (document.readyState === 'complete') {
    document.removeEventListener('readystatechange', pageReadyActions);
    numberOfFeeds = document.getElementsByClassName('WB_feed_type').length;
    //console.log('Feed number: ' + numberOfFeeds);
    var feedList = document.getElementsByClassName('WB_frame')[0];	// 'plc_main', 'WB_main_c', 'WB_feed'
    feedList.addEventListener('DOMSubtreeModified', feedListChange);
    addItemToMenuList();
  }
}

// Add list items when the feeds are refreshed or changed.
function feedListChange() {
  var num = document.getElementsByClassName('WB_feed_type').length;
  if (num !== numberOfFeeds) {
    numberOfFeeds = num;
    //console.log('Feed number: ' + numberOfFeeds);
    if (numberOfFeeds > 0) {
    	addItemToMenuList();
    }
  }
}

// Add a list item to each layer_menu_list.
function addItemToMenuList() {
  var screenBox = document.getElementsByClassName('screen_box');
  for (var i = 0; i < screenBox.length; ++i) {
    var menuList = screenBox[i].getElementsByClassName('layer_menu_list')[0];
    var targetList = menuList.getElementsByTagName('ul')[0];
    if (!targetList.children[0].textContent.includes('Get HTML')) {
      // Variables
      var newText = 'Get HTML';
      var newLink = document.createElement('a');
      newLink.setAttribute('class', 'GMS_getHtml_a');
      newLink.setAttribute('href', 'javascript:void(0);');
      newLink.setAttribute('title', newText);
      newLink.textContent = newText;
      newLink.addEventListener('click', getHtml);	// newLink.onclick = getHtml;
      var newItem = document.createElement('li');
      newItem.setAttribute('class', 'GMS_getHtml_li');
      newItem.appendChild(newLink);
      // Add
      targetList.prepend(newItem);
    }
  }
}

// Copy relevant HTML to clipboard.
function getHtml(e) {
  var node = e.target.parentNode;
  var level = 1;
  while (!node.className.includes('WB_cardwrap') && level < 12) {
    node = node.parentNode;
    ++level;
  }
  if (node.className.includes('WB_cardwrap')) {
    /*var comments = node.getElementsByClassName('WB_feed_repeat')[0];
    if (comments.style.getPropertyValue('display') === 'none') {	// comment section is not shown
      var feedDetail = node.getElementsByClassName('WB_feed_detail')[0];
    	GM.setClipboard(feedDetail.outerHTML);	// Don't use .replace(/\s+/g, ''), spaces may be in the text.
      console.log('Feed detail copied to clipboard.');
    } else {	// comment section is shown
      GM.setClipboard(node.outerHTML);
      console.log('Feed detail and comments copied to clipboard.');
    }*/
    GM.setClipboard(node.outerHTML, {type:'text', mimetype:'text/plain'});
    console.log('WB_cardwrap copied to clipboard.');
  } else {
    //console.log('WB_feed_detail not found or too far away.');
    console.log('WB_cardwrap not found or too far away.');
  }
}
