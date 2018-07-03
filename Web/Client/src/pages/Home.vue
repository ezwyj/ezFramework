<template>
	<div>
		<h1>VueWebTemplate</h1>
		<p>{{msg}}</p>
	</div>
</template>

<script>
import bus from '../bus';

export default {
	data() {
		return {
			msg: '数据绑定示例'
		};
	},
	created() {
		bus.pageNav = '首页232323';
	}
};
</script>
