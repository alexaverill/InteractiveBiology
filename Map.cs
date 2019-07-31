public class Map{
    private int _height;
    public int Height{
        get{ return _height;}
    }
    private int _width;
    public int Width{
        get{return _width;}
    }
    private int[,] currentMap;
    public int[,] MapRepresentation{
        get { return currentMap;}
    }
    public Map(int width, int height){
        _width = width;
        _height = height;
        currentMap = new int[_height,_width];
        generateMap();
    }
    private void generateMap(){
        
        for(int x=0; x<Height; x++){
            for(int y=0; y<Width; y++){
                currentMap[x,y] =0;
            }
        }
    }
}