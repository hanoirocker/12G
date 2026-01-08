NOVEDADES:


TOCHECK:
- Agregar tiempo de espera entre el dialogo primero raro en LockedUp y el que llama a Mica.
- Agregar trueno al bajar por las escaleras en Unwired?

TOFIX:
- No se están pausando las fuentes de lluvia al pausar el juego.
- El transform de los sonidos de garage cambia si se interactua con una puerta o algo, hay bobo.
- Si comienza un dialogo teniendo previamente la mano izq desocupada, y teniendo la derecha ocupada,
el jugador no ocupa sus ambas manos (puede interactuar).
- Se puede agarrar la linterna antes que termine la animación (agregar evento).
- Si habilitamos el Walkie Talkie mediante un checkpoint asset, se rompe al mostrarlo o intentar cambiar el canal debido a que no genera una estructura de canales. // Esto sucede cuando no existe extructura de audios de canales para el evento en particular //

TODO - próximo:
- Finalizar Visions y comenzar con RedHour

TODO - despues:

- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar sonido de mareo y caida del jugador al suelo, dependiendo del lugar donde caiga debe sonar distinto (3 materiales: carpet, wood, mosaic)
- Aumentar ganancia de tracks 12G - Door - Close (Hard) 2 y 12G - Door - Close (Hard) 1 ??
- Agregar animación de primera activación de linterna para Unwired? (MAYBE)
- Componer distintos audios según intensidad de Electric Feel. La idea es que sean clips conformados por el sonido actual (loopeable), luego un clip que incluya el layer anterior mas un fade in del nuevo layer de audio (Play one shot), y finalmente el nuevo audio (loopeable).
- Agregar sonidos stereo en loop para ambientación de cada habitación dentro de la casa??
  -> Se podrían disparar gracias al House Areas handler creado recientemente. El problema es que el volumen de audio sería el mismo en todos los puntos del volumen del collider.
- Agregar sonido para animación "Night - Stand Up".
- Agregar sonido para animación "Night - Unwired - Look For Flashlight" de FlashlightVC
- Agregar sonido de duda "Hum?" para transición de cámara a Focus enemy en Visions.

- Agregar cajas bajo la escalera, o algo.

- HEADACHES: actualmente se terminó el final de la primera parte, pero queda aún agregar lo que sucedería entre el final de Noises y el comienzo de Headaches donde choca el auto de la policia. Simon debería intentar contactarse con Mica luego de haber escuchado la conversación policial, pero sin lograrlo. Se podría mencionar el dolor de cabeza que le ocasiona el walkie talkie y los aparatos electrónicos. Quizas luego de eso podriamos prender y apagar aparatos aleatoriamente? (microondas, radio, luces de la casa)

- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

