import http from 'mpit-utils/libs/http';

export default {
	getList() {
		return http.get('Work/GetList');
	}
};