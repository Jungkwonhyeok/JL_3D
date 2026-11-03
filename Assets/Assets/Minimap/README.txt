소울나이트 방식 미니맵 UI (납작한 스타일)

배경    MM_Bg.png               256x256  Sliced 테두리 12
방 칸   MM_Room_Unvisited       48x48    아직 안 들어간 방 (어두운 회색)
        MM_Room_Visited         48x48    들어가 본 방 (밝은 회색)
        MM_Room_Current         48x48    현재 방 (더 밝게)  → 위에 MM_Cursor를 겹침
        ※ 모두 Sliced 테두리 12
커서    MM_Cursor.png           64x64    현재 방을 감싸는 흰색 모서리 (Sliced 테두리 20, Fill Center 해제)
        → 방 크기보다 가로세로 14px 크게 해서 방 위에 올림
통로    MM_Corridor_Unvisited / Visited  16x16  (두께 8~9px로 늘려서 사용)
아이콘  MM_Icon_Chest(보물상자) / Special(!) / Shop(상점) / Boss / Portal   64x64, 방 위에 24px 정도로 표시

임포트: Texture Type = Sprite (2D and UI), Mesh Type = Full Rect, Compression = None
색 규칙: 안 들어간 방 = Unvisited(어두움), 들어간 방 = Visited(밝음), 지금 있는 방 = Current + Cursor
        아이콘은 안 들어가 본 방에도 미리 보이게 해도 됩니다 (참고 이미지처럼)
