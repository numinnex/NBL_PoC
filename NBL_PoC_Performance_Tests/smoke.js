import http from 'k6/http'
import {sleep} from 'k6'
export let options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    vus: 3,
    duration: '1m'
}

const tenantIds = [1,2,3,4,5];
const API_BASE_URL = 'http://localhost:2137/';
export default ()  => {
    const randomTenantId = __VU % tenantIds.length;
    const currentTenantId = tenantIds[randomTenantId];
    const params = {
        headers: {
            'X-TenantId': currentTenantId,
            'api-version': 1,
        },
    }
    sleep(1);
    // Here some complex script scenario 
    // Step 1
    // Step 2
    // Step 3
};
    
