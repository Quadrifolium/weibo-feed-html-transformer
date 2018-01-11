# Weibo Feed HTML Transformer

Parses the HTML section of a Weibo feed/post into the format for use in [originalplan](https://github.com/Quadrifolium/originalplan) (GitHub Pages: [Original Plan](https://quadrifolium.github.io/originalplan/)).

## How-to

1. Install the .user.js in folder [gm_script](gm_script) to the extension [Greasemonkey](https://www.greasespot.net/) in a web browser.
1. Click the down arrow at the top right of a [Weibo](https://weibo.com/) feed/post, choose "Get HTML". This copies the HTML section to the clipboard of the operating system.
1. Run the UWP app "Weibo Feed HTML Transformer", click the button "Get HTML".
1. The result will be shown in the text box. This can be used for editing for the repository [originalplan](https://github.com/Quadrifolium/originalplan).

## Requirements

1. [Greasemonkey](https://www.greasespot.net/) for running the script to get HTML from a web brower.
1. [Html Agility Pack](http://html-agility-pack.net/) (HAP) for parsing HTML in C#.
