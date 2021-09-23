<template>
    <div>
        <zingchart  margin="dynamic" ref="tree"  :height="'70%'" :width="'100%'" :data="treeData" @shape_click="on_click" :series="series"></zingchart>
        <div>
        <vs-tooltip>
            <template #tooltip>
                Be sure to update the tree to see any changes
            </template>
            <button v-if="show" class="button" @click="update_tree">Update Tree</button>
        </vs-tooltip>
        </div>
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
            show: Boolean 
        },
        components:{
            zingchart : zingchartVue, 
        },
        data() {
        return {
            treeData: {
            plotarea: {
                margin: 50
            },
            type: 'tree',         
            options: {
                aspect: 'tree-down',
                orgChart: true,
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
                tooltip: {
                    padding: '8px 15px',
                    //text: '%data-type'
                }
                },
                link: {
                    aspect : 'sibling',
                    lineWidth: 2,
                    lineColor: 'grey', 

                }
            }
        }
        }
        }, 
        methods : {
            printnode : function(series){
                console.log(series)
            }, 
            get_children_id: function (parent_id) {
                var children_id=[]
                var idx = 0
                var idx_parent = 0
                for (idx=0; idx < this.series.length; idx++){
                    if (parent_id == this.series[idx].parent){
                        children_id.push({id: this.series[idx].id, name: this.series[idx].name} )
                    }
                    if (parent_id == this.series[idx].id){
                        idx_parent= idx
                    }
                }
                if(children_id.length===0){
                    children_id.push({name: this.series[idx_parent].name+"_weak" , id: parent_id,  level: "weak" })
                    children_id.push({name: this.series[idx_parent].name+"_medium", id: parent_id, level: "medium"})
                    children_id.push({name: this.series[idx_parent].name+"_strong", id: parent_id, level: "strong"})

                }
                var node_out = { node: parent_id, children: children_id }
                this.$emit('pass-children', node_out)
                console.log(node_out)
            },
            on_click : function(result){
                const id = result.shapeid.split('_');
                this.get_children_id(id[1])
                return id[1]
            }, 
        update_tree : function(){
        console.log( this.$refs.tree.getdata())
        try{
            this.$refs.tree.setdata({
                data : {
                    plotarea: {
                        margin: 50
                    },
                    type: 'tree',         
                    options: {
                        aspect: 'tree-down',
                        orgChart: true,
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
                        tooltip: {
                            padding: '8px 15px',
                        }
                        },
                        link: {
                            aspect : 'sibling',
                            lineWidth: 2,
                            lineColor: 'grey', 

                        },
                        
                    }, 
                    series : this.series
                    }}
            )
            
        console.log( this.$refs.tree.getdata())
        console.log("la methode zingchart a ete appele")
        this.$notify({
        group: 'foo',
        title: 'Update message',
        text: 'The tree has been updated successfully'
        });
        }catch{
        this.$notify({
            group: 'foo',
            title: 'Update message',
            text: 'Could not updtae the tree. Please refresh the page', 
            type: 'warn',
            duration: 1000,
        });  
        }
        }
    }}

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
.flat{
     margin: 0px;
     background: rgb(66, 66, 66)
}
#tooltip{
    font-family:inherit; 
}

</style>