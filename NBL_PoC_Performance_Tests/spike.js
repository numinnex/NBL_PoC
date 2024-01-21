import http from 'k6/http'
import {sleep} from 'k6'


export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        {duration: '10s', target: 100},
        {duration: '1m', target: 100},
        {duration: '10s', target: 1400},
        {duration: '3m', target: 1400},
        {duration: '10s', target: 100},
        {duration: '3m', target: 100},
        {duration: '10s', target: 0},
    ]
}

const tenantIds = [1,2,3,4,5];
const todoLen = 5;
const API_BASE_URL = 'http://localhost:2137/';
export default ()  => {
    const randomTenantId = __VU % tenantIds.length;
    const currentTenantId = tenantIds[randomTenantId];
    const todoId = (__VU % todoLen) + 1;
    const params = {
        headers: {
            'X-TenantId': currentTenantId,
            'api-version': 1,
        },
    }
    http.batch( [
        ['GET', API_BASE_URL + 'api/Todo', null, params],
        ['GET', API_BASE_URL + 'api/Todo/' + todoId, null, params],
    ]);

    sleep(1);
};
    
