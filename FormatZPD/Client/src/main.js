import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import Vuesax from 'vuesax'
import 'vuesax/dist/vuesax.css'
import { library } from '@fortawesome/fontawesome-svg-core'
import { fas} from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import Notifications from 'vue-notification'
Vue.use(Notifications)
library.add(fas)
Vue.component('font-awesome-icon', FontAwesomeIcon)
Vue.use(Vuesax)
Vue.config.productionTip = false;
new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount("#app");
