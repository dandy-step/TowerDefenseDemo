public static class UtilsClass {

	public static float MapFloat(float controlValue, float controlMin, float controlMax, float rangeMin, float rangeMax) {
        //maps value to a float range
        return (controlValue - controlMin) * (rangeMax - rangeMin) / (controlMax - controlMin) + rangeMin;
    }
}
