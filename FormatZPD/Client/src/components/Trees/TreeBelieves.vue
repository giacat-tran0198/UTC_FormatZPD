<template>
    <div>
        <zingchart ref="treeB" :height="400" :width="700" :data="treeData" :series="series"></zingchart>
         <button class="button" @click="update_tree">Update Tree</button>
    </div>
</template>

<script>
    import 'zingchart/es6';
    import 'zingchart/modules-es6/zingchart-tree.min.js';
    import zingchartVue from 'zingchart-vue';
    export default{
        name: 'Tree',
        props: {
            series: Array,
        }, 
        components:{
            zingchart : zingchartVue, 
        },
        data() {
        return {
            treeData: {
            type: 'tree',         
            options: {
                aspect: 'graph',
                springLength: 70,
                attractionConstant: 0.8,
                repulsionConstant: 200,
                repulsionDistanceFactor: 2,
    
                node: {
                type: 'circle',
                size: 20,
                borderWidth: 3,
                borderColor: 'black',
                backgroundColor: '#ccc',
                backgroundRepeat: 'no-repeat',
                backgroundScale: 0.5,
                label: {
                    color: 'black',
                    fontWeight: 'bold',
                    offsetY: 28
                },
                },
            }
        }
        }
        }, 
        methods: {
            update_tree : function(){
                console.log( this.$refs.treeB.getdata())
                this.$refs.treeB.setdata({
                data : {
                        type: 'tree',         
                        options: {
                            aspect: 'graph',
                            springLength: 70,
                            attractionConstant: 0.8,
                            repulsionConstant: 200,
                            repulsionDistanceFactor: 2,
                
                            node: {
                            type: 'circle',
                            size: 20,
                            borderWidth: 3,
                            borderColor: 'black',
                            backgroundColor: '#ccc',
                            backgroundRepeat: 'no-repeat',
                            backgroundScale: 0.5,
                            label: {
                                color: 'black',
                                fontWeight: 'bold',
                                offsetY: 28
                            },
                            },
                        },
                        series : this.series
                        }
                })
                
            console.log(this.$refs.treeB.getdata())
            console.log("la methode zingchart a ete appele")
            }
        }, 
        mounted(){
            console.log("Les believes du tree sont", this.series)
        }
    }

</script>

<style scoped>
 .button{
  display: inline-block;
  background: rgb(50,100,200)	;
  color : #fff; 
  border: 1px #0a1557;
  border-style: solid;
  padding: 5px 8px; 
  margin: 2px;
  border-radius: 10px;
  cursor: pointer;
  text-decoration: none;
  font-size:15px;
  font-family:inherit; 
}
.button:hover{
  background: #0a1557;	 
}
</style>