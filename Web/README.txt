VueWebTemplate项目模板说明：

一、概述
	本模板为基于ASP.NET MVC + Vue.js 2.x + Vue-Router + Element-UI架构实现的单页面应用。
	在此基础上实现了前后端分离，后端只提供数据接口，前端负责页面渲染和路由功能。

二、后端说明
	1、对路由RouteConfig.cs文件进行改造：
		(1)、对于/Api开头的路径交给后台处理，提供数据
		(2)、对于其他路径交给前端进行路由，页面渲染
	2、默认添加了两个Controller，其中ClientController为前端相关，CommonController模拟了用户信息及登录登出
	3、在Global.aspx中配置了log4net日志记录，并且添加了全局的错误处理和登录控制
	4、在开发阶段，与传统的ASP.NET MVC不同，后端和前端需要各自启http服务，后端的服务提供接口，前端的服务为基于Node.js启动的webpack-dev-serve服务，主要作用为热加载，自动刷新页面等。目前配置的前端端口为后端端口的数字+1，比如后端端口为59101，则前端端口为59102

三、前端说明
	1、前端需要安装Node.js和yarn
		(1)、安装Node.js，直接从 http://nodejs.cn/ 下载安装
		(2)、安装yarn，直接从 https://yarnpkg.com/zh-Hans/ 下载安装
	2、前端目录说明
		Client						代码根目录
		  |-- build					运行webpack进行构建代码所需的文件，其中的config.js文件包含构建的配置选项
									必须手动修改其中的配置
		  |-- src					项目源代码，开发者需要编辑的代码就写在这个目录下
		  |		|-- api				前端与后端对接的ajax请求，一般一个controller与api目录下的一个文件对应
		  |		|-- assets			页面中用到的静态资源，图片、音视频等
		  |		|-- libs			一些通用的js代码可封装到这里
		  |		|-- pages			路由页面
		  |		|-- styles			样式文件，其中veriables.scss为定制element-ui的变量，main.scss为一些通用样式，
		  |							页面自己的样式推荐写在页面中的style标签中
		  |		|-- bus.js			总线，作用是将一些通用的功能和跨页面数据存放在这里
		  |		|-- index.cshtml	页面入口，里面包含全局配置变量OP_CONFIG
									一般需要手动修改OP_CONFIG的配置以及页面title
		  |		|-- index.js		js入口文件
		  |		|-- router.js		路由配置
		  |-- static				静态资源目录，和src/assets不同，该目录的文件会直接复制到构建之后的文件夹中，
									不需要在js中通过require引用，一般用不上
		  |-- test					测试目录，目前基本用不上
		  |-- .babelrc				babel编译配置
		  |-- .editorconfig			统一编辑器配置
		  |-- .eslintrc.js			eslint校验配置
		  |-- package.json			包配置
		  |-- postcss.config.js		postcss配置
		  |-- yarn.lock				yarn生成的，无需手动编辑

	3、在Client目录中运行命令提示符或其他命令行工具：
		(1)、运行 yarn 命令安装前端依赖包
		(2)、运行 yarn dev 命令启动前端开发服务器并会自动打开浏览器，如果一切正常，会看到模拟登录页
		(3)、如果不正常则查看命令提示符工具的报错信息
	4、在发布阶段，运行 yarn build 命令
		(1)、会在根目录生成Static文件夹，里面包含前端所有经过编译的代码及资源文件
		(2)、会在根目录的Views中生成index.cshtml文件，该文件不能手动编辑
		(3)、在visual studio中将上述文件包含到项目中，然后发布
		(4)、拷贝到服务器时，一般只需要将Staitc和Views目录拷贝过去，不需要Client目录

四、技术文档
	1、Vue.js(https://cn.vuejs.org/v2/guide/)
	2、Vue-Router(https://router.vuejs.org/zh/)
	3、Element-UI(http://element.eleme.io/#/zh-CN/component/installation)
	4、字体图标库font-awesome(https://fontawesome.com/v4.7.0/icons/)